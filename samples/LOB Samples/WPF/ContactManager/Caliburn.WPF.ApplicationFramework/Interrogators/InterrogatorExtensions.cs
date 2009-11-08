namespace Caliburn.WPF.ApplicationFramework.Interrogators
{
    using ModelFramework;

    public static class InterrogatorExtensions
    {
        public static IPropertyDefinition<string> MustNotBeBlank(this IPropertyDefinition<string> model)
        {
            model.AddMetadata(new NotBlank());
            return model;
        }

        public static IPropertyDefinition<string> MustNotBeBlank(this IPropertyDefinition<string> model, string message)
        {
            model.AddMetadata(new NotBlank(message));
            return model;
        }

        public static IPropertyDefinition<T> MustNotBeNull<T>(this IPropertyDefinition<T> model)
            where T : class
        {
            model.AddMetadata(new NotNull<T>());
            return model;
        }

        public static IPropertyDefinition<T> MustNotBeNull<T>(this IPropertyDefinition<T> model, string message)
            where T : class
        {
            model.AddMetadata(new NotNull<T>(message));
            return model;
        }

        public static IPropertyDefinition<ICollectionNode<T>> HasMinimumCountOf<T>(this IPropertyDefinition<ICollectionNode<T>> model, int count)
        {
            model.AddMetadata(new MinimumCount<T>(count));
            return model;
        }

        public static IPropertyDefinition<ICollectionNode<T>> HasMinimumCountOf<T>(this IPropertyDefinition<ICollectionNode<T>> model, int count, string message)
        {
            model.AddMetadata(new MinimumCount<T>(message, count));
            return model;
        }

        public static IPropertyDefinition<T> MustMatch<T>(this IPropertyDefinition<T> model, string pattern)
            where T : class
        {
            model.AddMetadata(new Match( pattern));
            return model;
        }

        public static IPropertyDefinition<T> MustMatch<T>(this IPropertyDefinition<T> model, string pattern, string message)
            where T : class
        {
            model.AddMetadata(new Match(message, pattern));
            return model;
        }
    }
}