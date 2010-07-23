using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Caliburn
{
    using System.Linq;
    using System.Reflection;

    public class TestBase
    {
        [SetUp]
        public void SetUp()
        {
            given_the_context_of();
        }

        protected virtual void given_the_context_of()
        {
        }

        [TearDown]
        public void TearDown()
        {
            after_each();
        }

        protected virtual void after_each()
        {
        }

        protected T Mock<T>()
        {
            return MockRepository.GenerateMock<T>();
        }

        protected T StrictMock<T>()
        {
            var repository = new MockRepository();
            var strict = repository.StrictMock<T>();
            repository.Replay(strict);
            return strict;
        }

        protected T Stub<T>()
        {
            return MockRepository.GenerateStub<T>();
        }

        protected T Stub<T>(params object[] args)
        {
            return MockRepository.GenerateStub<T>(args);
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