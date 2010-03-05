namespace Caliburn.Core.Invocation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Threading;

    /// <summary>
    /// An implementation of <see cref="IMethodFactory"/>.
    /// </summary>
    public class DefaultMethodFactory : IMethodFactory
    {
        private readonly Dictionary<MethodInfo, IMethod> _cache = new Dictionary<MethodInfo, IMethod>();

        /// <summary>
        /// Creates an instance of <see cref="IMethod"/> using the <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>An instance of <see cref="IMethod"/>.</returns>
        public IMethod CreateFrom(MethodInfo methodInfo)
        {
            IMethod method;

            if(!_cache.TryGetValue(methodInfo, out method))
            {
                if(methodInfo.ReturnType == typeof(void))
                    method = new Procedure(methodInfo);
                else method = new Function(methodInfo);

                _cache[methodInfo] = method;
            }

            return method;
        }

        /// <summary>
        /// A base class for <see cref="IMethod"/> implementations.
        /// </summary>
        private abstract class MethodProxyBase : IMethod
        {
            private readonly MethodInfo _info;

            /// <summary>
            /// Initializes a new instance of the <see cref="MethodProxyBase"/> class.
            /// </summary>
            /// <param name="info">The info.</param>
            protected MethodProxyBase(MethodInfo info)
            {
                _info = info;
            }

            /// <summary>
            /// Gets the <see cref="MethodInfo"/> to which this instance applies.
            /// </summary>
            /// <value>The info.</value>
            public MethodInfo Info
            {
                get { return _info; }
            }

            /// <summary>
            /// Invokes the specified method on the provided instance with the given parameters.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="parameters">The parameters.</param>
            /// <returns>
            /// The result of the function or null if it is a procedure.
            /// </returns>
            public object Invoke(object instance, params object[] parameters)
            {
                return SafeInvoke(instance, parameters);
            }

            /// <summary>
            /// Creates a background task for executing this method asynchronously.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="parameters">The parameters.</param>
            /// <returns>
            /// An instance of <see cref="IBackgroundTask"/>.
            /// </returns>
            public IBackgroundTask CreateBackgroundTask(object instance, params object[] parameters)
            {
                return new BackgroundTask(() => SafeInvoke(instance, parameters));
            }

            protected abstract object SafeInvoke(object instance, object[] parameters);

            protected bool TryThrowParameterMismatch(object[] parameters)
            {
                var requirements = Info.GetParameters();

                if (requirements.Length != parameters.Length)
                {
                    throw new CaliburnException(
                        string.Format(
                            "The method '{0}' expected {1} parameters but was provided {2}.",
                            Info.Name,
                            requirements.Length,
                            parameters.Length
                            )
                        );
                }

                return false;
            }
        }

        /// <summary>
        /// A procedure implementation of <see cref="IMethod"/>.
        /// </summary>
        private class Procedure : MethodProxyBase
        {
            private readonly DelegateFactory.LateBoundProc _theDelegate;

            /// <summary>
            /// Initializes a new instance of the <see cref="Procedure"/> class.
            /// </summary>
            /// <param name="info">The info.</param>
            public Procedure(MethodInfo info)
                : base(info)
            {
                _theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundProc>(info);
            }

            protected override object SafeInvoke(object instance, object[] parameters)
            {
                try
                {
                    _theDelegate(instance, parameters);
                    return null;
                }
                catch (Exception)
                {
                    if(!TryThrowParameterMismatch(parameters))
                        throw;
                    return null;
                }
            }
        }

        /// <summary>
        /// A function implementation of <see cref="IMethod"/>.
        /// </summary>
        private class Function : MethodProxyBase
        {
            private readonly DelegateFactory.LateBoundFunc _theDelegate;

            /// <summary>
            /// Initializes a new instance of the <see cref="Function"/> class.
            /// </summary>
            /// <param name="info">The info.</param>
            public Function(MethodInfo info)
                : base(info)
            {
                _theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundFunc>(info);
            }

            protected override object SafeInvoke(object instance, object[] parameters)
            {
                try
                {
                    return _theDelegate(instance, parameters);
                }
                catch(Exception)
                {
                    if (!TryThrowParameterMismatch(parameters))
                        throw;
                    return null;
                }
            }
        }
    }
}