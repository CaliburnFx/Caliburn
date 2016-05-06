using Shouldly;

namespace Tests.Caliburn.Testability
{
    using Fakes.Model;
    using global::Caliburn.Testability;
    using Xunit;

    
    public class The_BoundType : TestBase
    {
        BoundType boundType;

        protected override void given_the_context_of()
        {
            boundType = new BoundType(typeof(MyPresenter));
        }

        [Fact]
        public void can_build_associated_BoundType_on_mixed_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = boundType.GetAssociatedType("Model");

            var type = associated.GetPropertyType("TypedSubModel");
            type.ShouldBe(typeof(MySubModel));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }

        [Fact]
        public void can_build_associated_BoundType_on_typed_paths_with_no_hints()
        {
            var associated = boundType.GetAssociatedType("TypedModel");

            var type = associated.GetPropertyType("TypedSubModel");
            type.ShouldBe(typeof(MySubModel));

            type = associated.GetPropertyType("TypedSubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }

        [Fact]
        public void can_build_associated_BoundType_on_untyped_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            var associated = boundType.GetAssociatedType("Model");


            var type = boundType.GetPropertyType("Model.SubModel");
            type.ShouldBe(typeof(MySubModel));

            type = boundType.GetPropertyType("Model.SubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }

        [Fact]
        public void can_resolve_mixed_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = boundType.GetPropertyType("Model.TypedSubModel");
            type.ShouldBe(typeof(MySubModel));

            type = boundType.GetPropertyType("Model.TypedSubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }

        [Fact]
        public void can_resolve_mutual_reference_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = boundType.GetPropertyType("Model.SubModel.Parent");
            type.ShouldBe(typeof(MyModel));
        }

        [Fact(Skip="Scenario not covered. Needs hints with associated 'starting type'")]
        public void can_resolve_mutual_reference_with_partial_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));
            boundType.AddHint("Model.SubModel.Parent", typeof(MyModel));

            var type = boundType.GetPropertyType("Model.TypedSubModel.Parent");
            type.ShouldBe(typeof(MyModel));
        }

        [Fact]
        public void can_resolve_typed_paths_with_no_hints()
        {
            var type = boundType.GetPropertyType("TypedModel");
            type.ShouldBe(typeof(MyModel));

            type = boundType.GetPropertyType("TypedModel.MyProperty");
            type.ShouldBe(typeof(string));

            type = boundType.GetPropertyType("TypedModel.TypedSubModel");
            type.ShouldBe(typeof(MySubModel));

            type = boundType.GetPropertyType("TypedModel.TypedSubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }

        [Fact]
        public void can_resolve_typed_paths_with_underscore_in_name()
        {
            var type = boundType.GetPropertyType("TypedModel.MyProperty_HasUnderscore");
            type.ShouldBe(typeof(string));
        }

        [Fact]
        public void can_resolve_untyped_paths_with_hints()
        {
            boundType.AddHint("Model", typeof(MyModel));
            boundType.AddHint("Model.SubModel", typeof(MySubModel));

            var type = boundType.GetPropertyType("Model");
            type.ShouldBe(typeof(MyModel));

            type = boundType.GetPropertyType("Model.MyProperty");
            type.ShouldBe(typeof(string));

            type = boundType.GetPropertyType("Model.SubModel");
            type.ShouldBe(typeof(MySubModel));

            type = boundType.GetPropertyType("Model.SubModel.MySubProperty");
            type.ShouldBe(typeof(int));
        }
    }
}