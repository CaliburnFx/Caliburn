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
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultMethodFactory));
        readonly Dictionary<MethodInfo, IMethod> cache = new Dictionary<MethodInfo, IMethod>();

        /// <summary>
        /// Creates an instance of <see cref="IMethod"/> using the <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>An instance of <see cref="IMethod"/>.</returns>
        public IMethod CreateFrom(MethodInfo methodInfo)
        {
            IMethod method;

            if(!cache.TryGetValue(methodInfo, out method))
            {
                if(methodInfo.ReturnType == typeof(void))
                    method = new Procedure(methodInfo);
                else method = new Function(methodInfo);

                Log.Info("Created method for {0}.", method);

                cache[methodInfo] = method;
            }

            return method;
        }

        private abstract class MethodProxyBase : IMethod
        {
            readonly MethodInfo info;

            protected MethodProxyBase(MethodInfo info)
            {
                this.info = info;
            }

            public MethodInfo Info
            {
                get { return info; }
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
            readonly DelegateFactory.LateBoundProc theDelegate;

            internal Procedure(MethodInfo info)
                : base(info)
            {
                theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundProc>(info);
            }

            protected override object InvokeCore(object instance, object[] parameters)
            {
                theDelegate(instance, parameters);
                return null;
            }
        }

        private class Function : MethodProxyBase
        {
            readonly DelegateFactory.LateBoundFunc theDelegate;

            internal Function(MethodInfo info)
                : base(info)
            {
                theDelegate = DelegateFactory.Create<DelegateFactory.LateBoundFunc>(info);
            }

            protected override object InvokeCore(object instance, object[] parameters)
            {
                return theDelegate(instance, parameters);
            }
        }
    }
}