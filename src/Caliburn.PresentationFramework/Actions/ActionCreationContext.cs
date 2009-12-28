namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.Invocation;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    public class ActionCreationContext
    {
        public ActionCreationContext(IServiceLocator serviceLocator, IMethodFactory methodFactory,
                                     IMessageBinder messageBinder, Type targetType, IFilterManager targetFilters,
                                     MethodInfo method)
        {
            ServiceLocator = serviceLocator;
            MethodFactory = methodFactory;
            MessageBinder = messageBinder;
            TargetType = targetType;
            TargetFilters = targetFilters;
            Method = method;
        }

        public Type TargetType { get; private set; }
        public IFilterManager TargetFilters { get; private set; }
        public MethodInfo Method { get; private set; }
        public IServiceLocator ServiceLocator { get; private set; }
        public IMethodFactory MethodFactory { get; private set; }
        public IMessageBinder MessageBinder { get; private set; }

        public void ApplyActionFilterConventions(IAction action, IMethod targetMethod)
        {
            var found = targetMethod.FindMetadata<PreviewAttribute>()
                .FirstOrDefault(x => x.MethodName == "Can" + targetMethod.Info.Name);

            if(found != null)
                return;

            var canExecute = targetMethod.Info.DeclaringType.GetMethod(
                                 "Can" + targetMethod.Info.Name,
                                 targetMethod.Info.GetParameters().Select(x => x.ParameterType).ToArray()
                                 )
                             ?? targetMethod.Info.DeclaringType.GetMethod("get_Can" + targetMethod.Info.Name);

            if(canExecute != null)
                action.Filters.Add(new PreviewAttribute(MethodFactory.CreateFrom(canExecute)));
        }
    }
}