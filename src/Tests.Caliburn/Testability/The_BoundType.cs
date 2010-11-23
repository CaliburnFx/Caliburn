namespace Tests.Caliburn.Testability
{
    using Fakes.Model;
    using global::Caliburn.Testability;
    using NUnit.Framework;


    [TestFixture]
    public class The_BoundType : TestBase
    {
        BoundType _boundType;

        protected override void given_the_context_of()
        {
            _boundType = new BoundType(typeof(MyPresenter));
        }

        [Test]
        public void can_build_associated_BoundType_on_mixed_paths_with_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = _boundType.GetAssociatedType("Model");

            var type = associated.GetPropertyType("TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_build_associated_BoundType_on_typed_paths_with_no_hints()
        {
            var associated = _boundType.GetAssociatedType("TypedModel");

            var type = associated.GetPropertyType("TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_build_associated_BoundType_on_untyped_paths_with_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = _boundType.GetAssociatedType("Model");


            var type = _boundType.GetPropertyType("Model.SubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = _boundType.GetPropertyType("Model.SubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_mixed_paths_with_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = _boundType.GetPropertyType("Model.TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = _boundType.GetPropertyType("Model.TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_mutual_reference_with_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));
            _boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = _boundType.GetPropertyType("Model.SubModel.Parent");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));
        }

        [Test, Ignore("Scenario not covered. Needs hints with associated 'starting type'")]
        public void can_resolve_mutual_reference_with_partial_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));
            _boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = _boundType.GetPropertyType("Model.TypedSubModel.Parent");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));
        }

        [Test]
        public void can_resolve_typed_paths_with_no_hints()
        {
            var type = _boundType.GetPropertyType("TypedModel");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));

            type = _boundType.GetPropertyType("TypedModel.MyProperty");
            Assert.That(type, Is.EqualTo(typeof(string)));

            type = _boundType.GetPropertyType("TypedModel.TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = _boundType.GetPropertyType("TypedModel.TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_typed_paths_with_underscore_in_name()
        {
            var type = _boundType.GetPropertyType("TypedModel.MyProperty_HasUnderscore");
            Assert.That(type, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void can_resolve_untyped_paths_with_hints()
        {
            _boundType.AddHint("Model", typeof(MyModel));
            _boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = _boundType.GetPropertyType("Model");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));

            type = _boundType.GetPropertyType("Model.MyProperty");
            Assert.That(type, Is.EqualTo(typeof(string)));

            type = _boundType.GetPropertyType("Model.SubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = _boundType.GetPropertyType("Model.SubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }
    }
}