namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework;
    using PresentationFramework.Behaviors;
    using PresentationFramework.Screens;

    /// <summary>
    /// Handles <see cref="IScreen"/> members.
    /// </summary>
    public class ScreenInterceptor : InterceptorBase
    {
        private readonly ScreenAttribute _attribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenInterceptor"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        public ScreenInterceptor(ScreenAttribute attribute)
        {
            _attribute = attribute;
        }

        /// <summary>
        /// Initializes the interceptor with the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public override void Initialize(object proxy)
        {
            var notifier = proxy as INotifyPropertyChangedEx;
            if (notifier != null)
                notifier.PropertyChanged += (s, e) =>{
                    if(e.PropertyName == _attribute.DisplayName)
                        notifier.NotifyOfPropertyChange("DisplayName");
                };
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            if (invocation.Method.DeclaringType.Equals(typeof(IScreen)))
            {
                switch(invocation.Method.Name)
                {
                    case "get_DisplayName":
                        GetMethod(invocation.Proxy, "get_" + _attribute.DisplayName, info =>{
                            invocation.ReturnValue = info.Invoke(invocation.Proxy, null);
                        });
                        break;
                    case "set_DisplayName":
                        GetMethod(invocation.Proxy, "set_" + _attribute.DisplayName,
                                  info => info.Invoke(invocation.Proxy, invocation.Arguments)
                            );
                        break;
                    case "Initialize":
                        ExecuteLifecycleMethod(invocation.Proxy, _attribute.Initialize);
                        break;
                    case "CanShutdown":
                        if (!GetMethod(invocation.Proxy, _attribute.CanShutdown, info =>
                        {
                            invocation.ReturnValue = info.Invoke(invocation.Proxy, GetArgs(info));
                        }))
                        {
                            invocation.ReturnValue = true;
                        }
                        break;
                    case "Shutdown":
                        ExecuteLifecycleMethod(invocation.Proxy, _attribute.Shutdown);
                        break;
                    case "Activate":
                        ExecuteLifecycleMethod(invocation.Proxy, _attribute.Activate);
                        break;
                    case "Deactivate":
                        ExecuteLifecycleMethod(invocation.Proxy, _attribute.Deactivate);
                        break;
                }
            }
            else invocation.Proceed();
        }

        private static void ExecuteLifecycleMethod(object proxy, string methodName)
        {
            GetMethod(proxy, methodName, info =>{
                var args = GetArgs(info);
                var result = info.Invoke(proxy, args);

                var iResult = result as IResult;
                if(iResult != null)
                    iResult.ExecuteFor(proxy);
                else
                {
                    var enumerableResult = result as IEnumerable<IResult>;
                    if(enumerableResult != null)
                        enumerableResult.ExecuteFor(proxy);
                }
            });
        }

        private static object[] GetArgs(MethodInfo info)
        {
            var locator = ServiceLocator.Current;
            return (from parameter in info.GetParameters()
                    select locator.GetInstance(parameter.ParameterType)).ToArray();
        }

        private static bool GetMethod(object proxy, string methodName, Action<MethodInfo> borrow)
        {
            var method = proxy.GetType().GetMethod(methodName);
            if (method == null) return false;

            borrow(method);
            return true;
        }
    }
}