namespace Tests.Caliburn.Testability
{
    using Fakes.Model;
    using global::Caliburn.Testability;
    using NUnit.Framework;

    [TestFixture]
    public class The_BoundType : TestBase
    {
        BoundType boundType;

        protected override void given_the_context_of()
        {
            boundType = new BoundType(typeof(MyPresenter));
        }

        [Test]
        public void can_build_associated_BoundType_on_mixed_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = boundType.GetAssociatedType("Model");

            var type = associated.GetPropertyType("TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_build_associated_BoundType_on_typed_paths_with_no_hints()
        {
            var associated = boundType.GetAssociatedType("TypedModel");

            var type = associated.GetPropertyType("TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_build_associated_BoundType_on_untyped_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = boundType.GetAssociatedType("Model");


            var type = boundType.GetPropertyType("Model.SubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = boundType.GetPropertyType("Model.SubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_mixed_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = boundType.GetPropertyType("Model.TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = boundType.GetPropertyType("Model.TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_mutual_reference_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = boundType.GetPropertyType("Model.SubModel.Parent");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));
        }

        [Test, Ignore("Scenario not covered. Needs hints with associated 'starting type'")]
        public void can_resolve_mutual_reference_with_partial_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = boundType.GetPropertyType("Model.TypedSubModel.Parent");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));
        }

        [Test]
        public void can_resolve_typed_paths_with_no_hints()
        {
            var type = boundType.GetPropertyType("TypedModel");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));

            type = boundType.GetPropertyType("TypedModel.MyProperty");
            Assert.That(type, Is.EqualTo(typeof(string)));

            type = boundType.GetPropertyType("TypedModel.TypedSubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = boundType.GetPropertyType("TypedModel.TypedSubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void can_resolve_typed_paths_with_underscore_in_name()
        {
            var type = boundType.GetPropertyType("TypedModel.MyProperty_HasUnderscore");
            Assert.That(type, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void can_resolve_untyped_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = boundType.GetPropertyType("Model");
            Assert.That(type, Is.EqualTo(typeof(MyModel)));

            type = boundType.GetPropertyType("Model.MyProperty");
            Assert.That(type, Is.EqualTo(typeof(string)));

            type = boundType.GetPropertyType("Model.SubModel");
            Assert.That(type, Is.EqualTo(typeof(MySubModel)));

            type = boundType.GetPropertyType("Model.SubModel.MySubProperty");
            Assert.That(type, Is.EqualTo(typeof(int)));
        }
    }
}