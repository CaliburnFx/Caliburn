using System;
using NSubstitute;

namespace Tests.Caliburn
{
    using System.Linq;
    using System.Reflection;

    public class TestBase : IDisposable
    {
        public TestBase()
        {
            given_the_context_of();
        }

        protected virtual void given_the_context_of()
        {
        }

        public void Dispose()
        {
            after_each();
        }

        protected virtual void after_each()
        {
        }

        protected T Mock<T>() where T : class
        {
            return Substitute.For<T>();
        }

        protected void CallProc(object instance, string name, params object[] parameters)
        {
            var type = instance.GetType();
            var proc = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .FirstOrDefault(x =>{
                    var index = x.Name.LastIndexOf(".");
                    if (index == -1)
                        index = 0;

                    var methodName = x.Name.Substring(index + 1);

                    return name == methodName;
                });

            proc.Invoke(instance, parameters);
        }
    }
}
