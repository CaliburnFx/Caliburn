namespace Caliburn.ShellFramework
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Core;
    using Core.Configuration;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using Questions;

    /// <summary>
    /// General extension methods related to the shell framework.
    /// </summary>
    public static class ShellFrameworkExtensions
    {
        static Func<IQuestionDialog> createQuestionDialog = () => IoC.Get<IQuestionDialog>();

        /// <summary>
        /// Initializes the specified dialog factory.
        /// </summary>
        /// <param name="dialogFactory">The dialog factory.</param>
        /// <remarks>Useful during unit testing.</remarks>
        public static void Initialize(Func<IQuestionDialog> dialogFactory)
        {
            createQuestionDialog = dialogFactory;
        }

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="text">The text.</param>
        public static void ShowMessageBox(this IWindowManager windowManager, string text)
        {
            windowManager.ShowMessageBox(text, "Info", null, null);
        }

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        public static void ShowMessageBox(this IWindowManager windowManager, string text, string caption)
        {
            windowManager.ShowMessageBox(text, caption, null, null);
        }

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="callback">The answer callback.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public static void ShowMessageBox(this IWindowManager windowManager, string text, string caption, Action<Answer> callback, params Answer[] possibleAnswers)
        {
            if (possibleAnswers == null || possibleAnswers.Length < 1)
                possibleAnswers = new[] { Answer.Ok };

            var questionDialog = createQuestionDialog();
            var question = new Question(text, possibleAnswers);

            questionDialog.Setup(caption, new[]{question});

            if(callback != null)
            {
                EventHandler<DeactivationEventArgs> handler = null;
                handler = (s, e) =>{
                    if(!e.WasClosed)
                        return;

                    questionDialog.Deactivated -= handler;
                    callback(question.Answer);
                };
                questionDialog.Deactivated += handler;
            }

            windowManager.ShowDialog(questionDialog, null);
        }

        /// <summary>
        /// Adds the shell framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static ShellFrameworkConfiguration ShellFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<ShellFrameworkConfiguration>.Instance);
        }

        /// <summary>
        /// Determines the display name for the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string DetermineDisplayName(this object item)
        {
            var displayNamed = item as IHaveDisplayName;
            if(displayNamed != null)
                return displayNamed.DisplayName;
            return item != null ? item.ToString() : string.Empty;
        }

        /// <summary>
        /// Creates the scope.
        /// </summary>
        /// <typeparam name="TScope">The type of scope controller.</typeparam>
        /// <typeparam name="TInScope">The type of the item in scope.</typeparam>
        /// <param name="inScope">The in item scope.</param>
        /// <param name="scope">The scope controller.</param>
        /// <returns></returns>
        public static Scope<TScope, TInScope> CreateScope<TScope, TInScope>(this TInScope inScope, TScope scope)
            where TInScope : IList<TInScope>, IChild<TInScope>
            where TScope : IActivate, IDeactivate
        {
            return new Scope<TScope, TInScope>(scope);
        }

        /// <summary>
        /// Returns the descendants and self.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <param name="self">The item to return descendants for.</param>
        /// <returns>The flattened hierarchy.</returns>
        public static IEnumerable<T> DescendantsAndSelf<T>(this T self)
            where T : IEnumerable<T>
        {
            var queue = new Queue<T>(self);

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();
                current.Apply(queue.Enqueue);
                yield return current;
            }
        }
    }
}