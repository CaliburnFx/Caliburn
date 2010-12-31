namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Validates a <see cref="TriggerCollection"/>.
    /// </summary>
    public class TriggerValidator
    {
        readonly IElement item;
        readonly BoundType type;
        readonly TriggerCollection triggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="item">The item.</param>
        /// <param name="triggers">The triggers.</param>
        public TriggerValidator(BoundType type, IElement item, TriggerCollection triggers)
        {
            this.item = item;
            this.type = type;
            this.triggers = triggers;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public IElement Item
        {
            get { return item; }
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        public TriggerCollection Triggers
        {
            get { return triggers; }
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns></returns>
        public ValidationResult ValidateBindings()
        {
            var result = new ValidationResult();

            foreach(var info in GetBindings())
            {
                var validatedProperty = Type.ValidateAgainst(item, info.Property, info.Binding);
                result.Add(validatedProperty);
            }

            return result;
        }

        IEnumerable<BindingInfo> GetBindings()
        {
            foreach(var trigger in triggers)
            {
                var dataTrigger = trigger as DataTrigger;

                if(dataTrigger != null)
                {
                    foreach(var binding in dataTrigger.Binding.GetActualBindings())
                    {
                        yield return new BindingInfo(binding, null);
                    }
                }
                else
                {
                    var multiTrigger = trigger as MultiDataTrigger;

                    if(multiTrigger != null)
                    {
                        foreach(var condition in multiTrigger.Conditions)
                        {
                            foreach(var binding in condition.Binding.GetActualBindings())
                            {
                                yield return new BindingInfo(binding, condition.Property);
                            }
                        }
                    }
                }
            }
        }
    }
}