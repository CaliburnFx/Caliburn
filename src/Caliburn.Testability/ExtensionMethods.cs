namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Windows.Data;

    /// <summary>
    /// Extension methods related to testability.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Examines a <see cref="BindingBase"/> and returns the actual bindings.
        /// </summary>
        /// <param name="bindingBase">The binding base.</param>
        /// <returns></returns>
        public static IEnumerable<Binding> GetActualBindings(this BindingBase bindingBase)
        {
            var binding = bindingBase as Binding;

            if(binding.ShouldValidate()) yield return binding;
            else
            {
                var multiBinding = bindingBase as MultiBinding;

                if(multiBinding != null)
                {
                    foreach(var child in multiBinding.Bindings)
                    {
                        foreach(var actualBinding in child.GetActualBindings())
                        {
                            yield return actualBinding;
                        }
                    }
                }
                else
                {
                    var priorityBinding = bindingBase as PriorityBinding;

                    if(priorityBinding != null)
                    {
                        foreach(var child in priorityBinding.Bindings)
                        {
                            foreach(var actualBinding in child.GetActualBindings())
                            {
                                yield return actualBinding;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the actual bindings from a <see cref="BindingExpressionBase"/>.
        /// </summary>
        /// <param name="expressionBase">The expression base.</param>
        /// <returns></returns>
        public static IEnumerable<Binding> GetActualBindings(this BindingExpressionBase expressionBase)
        {
            var bindingExpression = expressionBase as BindingExpression;

            if(bindingExpression != null &&
               bindingExpression.ParentBinding.ShouldValidate()) yield return bindingExpression.ParentBinding;
            else
            {
                var multiBindingExpression = expressionBase as MultiBindingExpression;

                if(multiBindingExpression != null)
                {
                    foreach(var child in multiBindingExpression.ParentMultiBinding.Bindings)
                    {
                        foreach(var actualBinding in child.GetActualBindings())
                        {
                            yield return actualBinding;
                        }
                    }
                }
                else
                {
                    var priorityBindingExpression = expressionBase as PriorityBindingExpression;

                    if(priorityBindingExpression != null)
                    {
                        foreach(var child in priorityBindingExpression.ParentPriorityBinding.Bindings)
                        {
                            foreach(var actualBinding in child.GetActualBindings())
                            {
                                yield return actualBinding;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the binding should be validated.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static bool ShouldValidate(this Binding binding)
        {
            return binding != null && binding.RelativeSource == null &&
                   string.IsNullOrEmpty(binding.ElementName) && binding.Source == null &&
                   binding.Path != null && !string.IsNullOrEmpty(binding.Path.Path);
        }
    }
}