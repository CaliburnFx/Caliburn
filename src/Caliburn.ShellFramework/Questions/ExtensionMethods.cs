namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using Core;
    using PresentationFramework.ApplicationModel;

    public static class ExtensionMethods
    {
        public static IEnumerable<Question> GetQuestions(this ISubordinate model)
        {
            var queue = new Queue<ISubordinate>();
            queue.Enqueue(model);

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();
                var anotherCompoiste = current as ISubordinateComposite;

                if(anotherCompoiste != null)
                    anotherCompoiste.GetChildren().Apply(queue.Enqueue);
                else yield return (Question)current;
            }
        }
    }
}