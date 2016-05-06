namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using Conventions;
    using Core;
    using System.Linq;
    using Core.Logging;

    /// <summary>
    /// The default implementation of <see cref="IMessageBinder"/>.
    /// </summary>
    public class DefaultMessageBinder : IMessageBinder
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultMessageBinder));

        readonly IConventionManager conventionManager;
        readonly IDictionary<string, Func<IInteractionNode, object, object>> valueHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageBinder"/> class.
        /// </summary>
        public DefaultMessageBinder(IConventionManager conventionManager)
        {
            this.conventionManager = conventionManager;
            valueHandlers = new Dictionary<string, Func<IInteractionNode, object, object>>();
            
            InitializeDefaultValueHandlers();
        }

        /// <summary>
        /// Determines whether the supplied value is recognized as a specialy treated value.
        /// </summary>
        /// <param name="potential">The potential value.</param>
        /// <returns>
        /// 	<c>true</c> if a special value; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsSpecialValue(string potential)
        {
            if(string.IsNullOrEmpty(potential))
                return false;

            if (!potential.StartsWith("$"))
                return false;

            var splits = potential.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var normalized = splits[0].TrimStart('$').ToLower();

            return valueHandlers.ContainsKey(normalized);
        }

        /// <summary>
        /// Identifies a special value along with its handler.
        /// </summary>
        /// <param name="specialValue">The special value.</param>
        /// <param name="handler">The handler.</param>
        public void AddValueHandler(string specialValue, Func<IInteractionNode, object, object> handler)
        {
            valueHandlers[specialValue.TrimStart('$').ToLower()] = handler;
        }

        /// <summary>
        /// Determines the parameters that a method should be invoked with.
        /// </summary>
        /// <param name="message">The message to determine the parameters for.</param>
        /// <param name="requiredParameters">The requirements for method binding.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="context">The context.</param>
        /// <returns>The actual parameter values.</returns>
        public virtual object[] DetermineParameters(IRoutedMessage message, IList<RequiredParameter> requiredParameters, IInteractionNode handlingNode, object context)
        {
            if (requiredParameters == null || requiredParameters.Count == 0)
                return new object[] {};

            var providedValues = message.Parameters.Select(x => x.Value).ToArray();

            if (requiredParameters.Count <= providedValues.Length)
                return CoerceValues(requiredParameters, providedValues, message.Source, context);

            if (providedValues.Length == 0)
                return LocateAndCoerceValues(requiredParameters, message.Source, handlingNode, context);

            var exception = new CaliburnException(
                string.Format(
                    "Parameter count mismatch.  {0} parameters were provided but {1} are required for {2}.",
                    providedValues.Length,
                    requiredParameters.Count,
                    message
                    )
                );

            Log.Error(exception);
            throw exception;
        }

        /// <summary>
        /// Binds the return value to the UI.
        /// </summary>
        /// <param name="outcome">The outcome or processing the message.</param>
        public virtual IResult CreateResult(MessageProcessingOutcome outcome)
        {
            if (outcome.ResultType == typeof(void))
                return new EmptyResult();

            if(outcome.Result is IEnumerable<IResult>)
                return new SequentialResult(((IEnumerable<IResult>)outcome.Result).GetEnumerator());

            if (outcome.Result is IEnumerator<IResult>)
                return new SequentialResult((IEnumerator<IResult>)outcome.Result);

            if (outcome.Result is IResult)
                return new SequentialResult(new List<IResult>{ (IResult)outcome.Result }.GetEnumerator());

            return new DefaultResult(conventionManager, outcome);
        }

        /// <summary>
        /// Locates and perofrms type coercion for the required parameters.
        /// </summary>
        /// <param name="requiredParameters">The required parameters.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object[] LocateAndCoerceValues(IList<RequiredParameter> requiredParameters, IInteractionNode sourceNode, IInteractionNode handlingNode, object context)
        {
            var values = new object[requiredParameters.Count];

            for (int i = 0; i < requiredParameters.Count; i++)
            {
                var parameter = requiredParameters[i];
                object value;

                if (!DetermineSpecialValue(parameter.Name.ToLower(), sourceNode, context, out value))
                {
                    var control = handlingNode.UIElement.FindNameExhaustive<DependencyObject>(parameter.Name, true);
                    var convention = conventionManager.FindElementConventionOrFail(control);
                    value = convention.GetValue(control);
                }

                values[i] = CoerceParameter(parameter, value);
            }

            return values;
        }

        /// <summary>
        /// Coerces the values.
        /// </summary>
        /// <param name="requiredParameters">The required parameters.</param>
        /// <param name="providedValues">The provided values.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object[] CoerceValues(IList<RequiredParameter> requiredParameters, object[] providedValues, IInteractionNode sourceNode, object context)
        {
            var values = new object[requiredParameters.Count];

            for (int i = 0; i < requiredParameters.Count; i++)
            {
                var possibleSpecialValue = providedValues[i] as string;

                if (possibleSpecialValue != null)
                    providedValues[i] = ResolveSpecialValue(possibleSpecialValue, sourceNode, context);

                values[i] = CoerceParameter(requiredParameters[i], providedValues[i]);
            }

            return values;
        }

        /// <summary>
        /// Determines if the key is a special value.
        /// </summary>
        /// <param name="possibleKey">The possible key.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual bool DetermineSpecialValue(string possibleKey, IInteractionNode sourceNode, object context, out object value)
        {
            Func<IInteractionNode, object, object> handler;
 
            if(valueHandlers.TryGetValue(possibleKey, out handler))
            {
                value = handler(sourceNode, context);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Resolves the special value.
        /// </summary>
        /// <param name="potential">The possible special value.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual object ResolveSpecialValue(string potential, IInteractionNode sourceNode, object context)
        {
            var splits = potential.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (splits.Length == 0)
                return potential;

            var normalized = splits[0].TrimStart('$').ToLower();
            
            object value;
            if(DetermineSpecialValue(normalized, sourceNode, context, out value))
            {
                if(splits.Length > 1)
                {
                    var getter = CreateGetter(value, splits.Skip(1).ToArray());
                    return getter();
                }

                return value;
            }

            return potential;
        }

        /// <summary>
        /// Coerces the parameter.
        /// </summary>
        /// <param name="parameter">The required parameter.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <returns></returns>
        protected virtual object CoerceParameter(RequiredParameter parameter, object providedValue)
        {
            return CoerceValueCore(
                parameter.Type,
                providedValue
                );
        }

        /// <summary>
        /// Coerces the provided value to the destination type.
        /// </summary>
        /// <param name="destinationType">The destination type.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <returns>The coerced value.</returns>
        public static object CoerceValueCore(Type destinationType, object providedValue)
        {
            if (providedValue == null) return GetDefaultValue(destinationType);

            var providedType = providedValue.GetType();

            if (destinationType.IsAssignableFrom(providedType))
                return providedValue;

            try
            {
                var converter = TypeDescriptor.GetConverter(destinationType);

                if (converter.CanConvertFrom(providedType))
                    return converter.ConvertFrom(providedValue);

                converter = TypeDescriptor.GetConverter(providedType);

                if (converter.CanConvertTo(destinationType))
                    return converter.ConvertTo(providedValue, destinationType);

                if (destinationType.IsEnum) {
                    var stringValue = providedValue as string;
                    if(stringValue != null)
                        return Enum.Parse(destinationType, stringValue, true);
                    return Enum.ToObject(destinationType, providedValue);
                }
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }

            try
            {
                return Convert.ChangeType(providedValue, destinationType, CultureInfo.CurrentUICulture);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
        }

        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public static object GetDefaultValue(Type type)
        {
            return type.IsClass || type.IsInterface ? null : Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates the default value handlers.
        /// </summary>
        /// <returns></returns>
        void InitializeDefaultValueHandlers()
        {
            AddValueHandler("eventargs", HandleContext);
            AddValueHandler("parameter", HandleContext);
            AddValueHandler("source", HandleSource);
            AddValueHandler("datacontext", HandleDataContext);
            AddValueHandler("value", HandleValue);
        }

        object HandleContext(IInteractionNode sourceNode, object context)
        {
            return context;
        }

        object HandleSource(IInteractionNode sourceNode, object context)
        {
            return sourceNode.UIElement;
        }

        object HandleDataContext(IInteractionNode sourceNode, object context)
        {
            return sourceNode.UIElement.GetDataContext();
        }

        object HandleValue(IInteractionNode sourceNode, object context)
        {
            var ele = sourceNode.UIElement;
            var defaults = conventionManager.FindElementConventionOrFail(ele);
            return defaults.GetValue(ele);
        }

        /// <summary>
        /// Finds the property setter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        protected virtual Func<object> CreateGetter(object target, string[] propertyPath)
        {
            PropertyInfo propertyInfo;

            for(int i = 0; i < propertyPath.Length; i++)
            {
                string propertyName = propertyPath[i];
                propertyInfo = target.GetType().GetProperty(propertyName);

                if(propertyInfo == null)
                    throw new CaliburnException(
                        string.Format("{0} is not a valid property path.", propertyPath.Aggregate((a, c) => a + c))
                        );

                if(i < propertyPath.Length - 1)
                {
                    target = propertyInfo.GetValue(target, null);
                    if(target == null) return () => null;
                }
                else
                {
                    return () => propertyInfo.GetValue(
                                     target,
                                     null
                                     );
                }
            }

            return () => null;
        }

        private class EmptyResult : IResult
        {
            public void Execute(ResultExecutionContext context)
            {
                Completed(this, new ResultCompletionEventArgs());
            }

            public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
        }
    }
}