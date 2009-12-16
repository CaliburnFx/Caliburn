using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Caliburn
{
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
    }
}