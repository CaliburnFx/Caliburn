namespace Caliburn.Core.Invocation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Logging;

    /// <summary>
    /// An implementation of <see cref="IMethodFactory"/>.
    /// </summary>
    public class DefaultMethodFactory : IMethodFactory
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultMethodFactory));
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

                Log.Info("Created method for {0}.", method);

                _cache[methodInfo] = method;
            }

            return method;
        }

        private abstract class MethodProxyBase : IMethod
        {
            private readonly MethodInfo _info;

            protected MethodProxyBase(MethodInfo info)
            {
                _info = info;
            }

            public MethodInfo Info
            {
                get { return _info; }
            }

            public object Invoke(object instance, params object[] parameters)
            {
                return SafeInvoke(instance, parameters);
            }

            public IBackgroundTask CreateBackgroundTask(object instance, params object[] parameters)
            {
                return new BackgroundTask(() => SafeInvoke(instance, parameters));
            }

            private object SafeInvoke(object instance, object[] parameters)
            {
                try
                {
                    return InvokeCore(instance, parameters);
                }
                catch (Exception exception)
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
                                ),
                            exception
                            );
                    }

                    throw;
                }
            }

            protected abstract object InvokeCore(object instance, object[] parameters);
        }

        private class Procedure : MethodProxyBase
        {
            private readonly DelegateFactory.LateBoundProc _theDelegate;

            internal Procedure(MethodInfo info)
                : base(info)
            {
                _theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundProc>(info);
            }

            protected override object InvokeCore(object instance, object[] parameters)
            {
                _theDelegate(instance, parameters);
                return null;
            }
        }

        private class Function : MethodProxyBase
        {
            private readonly DelegateFactory.LateBoundFunc _theDelegate;

            internal Function(MethodInfo info)
                : base(info)
            {
                _theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundFunc>(info);
            }

            protected override object InvokeCore(object instance, object[] parameters)
            {
                return _theDelegate(instance, parameters);
            }
        }
    }
}