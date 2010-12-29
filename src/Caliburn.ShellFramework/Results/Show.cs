namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq.Expressions;
    using Core;
    using Microsoft.Win32;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using Questions;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Factory for display-related <see cref="IResult"/> instances.
    /// </summary>
    public static class Show
    {
        /// <summary>
        /// Shows a child based on a <see cref="ISubjectSpecification"/>.
        /// </summary>
        /// <param name="subjectSpecification">The subject specification.</param>
        /// <returns>The result.</returns>
        public static OpenScreenSubjectResult ChildSubject(ISubjectSpecification subjectSpecification)
        {
            return new OpenScreenSubjectResult(subjectSpecification);
        }

        /// <summary>
        /// Shows the child subject based on the default specification.
        /// </summary>
        /// <typeparam name="T">The subject type.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The result.</returns>
        public static OpenScreenSubjectResult ChildFor<T>(T subject)
        {
            return ChildSubject(new SubjectSpecification<T>(subject));
        }

        /// <summary>
        /// Shows a child in an <see cref="IConductor"/>.
        /// </summary>
        /// <typeparam name="TChild">The type of the child ViewModel.</typeparam>
        /// <returns>The result.</returns>
        public static OpenChildResult<TChild> Child<TChild>()
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>();
        }

        /// <summary>
        /// Shows the specified child in an <see cref="IConductor"/>.
        /// </summary>
        /// <typeparam name="TChild">The type of the child ViewModel.</typeparam>
        /// <param name="child">The child.</param>
        /// <returns>The result.</returns>
        public static OpenChildResult<TChild> Child<TChild>(TChild child)
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>(child);
        }

        /// <summary>
        /// Shows a modal dialog base on an <see cref="ISubjectSpecification"/>.
        /// </summary>
        /// <param name="subjectSpecification">The subject specification.</param>
        /// <returns>The result.</returns>
        public static DialogScreenSubjectResult DialogSubject(ISubjectSpecification subjectSpecification)
        {
            return new DialogScreenSubjectResult(subjectSpecification);
        }

        /// <summary>
        /// Shows the subject as a modal dialog based on the default specification.
        /// </summary>
        /// <typeparam name="T">The type of the subject.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The result.</returns>
        public static DialogScreenSubjectResult DialogFor<T>(T subject)
        {
            return new DialogScreenSubjectResult(new SubjectSpecification<T>(subject));
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <typeparam name="TModel">The type of the dialog ViewModel.</typeparam>
        /// <returns>The result</returns>
        public static OpenDialogResult<TModel> Dialog<TModel>()
            where TModel : IScreen
        {
            return new OpenDialogResult<TModel>();
        }

        /// <summary>
        /// Shows a modal dialog for the specified ViewModel.
        /// </summary>
        /// <typeparam name="TModel">The type of the ViewModel.</typeparam>
        /// <param name="model">The ViewModel.</param>
        /// <returns>The result.</returns>
        public static OpenDialogResult<TModel> Dialog<TModel>(TModel model)
            where TModel : IScreen
        {
            return new OpenDialogResult<TModel>(model);
        }

        /// <summary>
        /// Shows a popup.
        /// </summary>
        /// <typeparam name="TModel">The type of the ViewModel.</typeparam>
        /// <returns>The result.</returns>
        public static PopupResult<TModel> Popup<TModel>()
        {
            return new PopupResult<TModel>();
        }

        /// <summary>
        /// Shows a popup for the specified ViewModel.
        /// </summary>
        /// <typeparam name="TModel">The type of the ViewMOdel.</typeparam>
        /// <param name="model">The ViewModel.</param>
        /// <returns>The result.</returns>
        public static PopupResult<TModel> Popup<TModel>(TModel model)
        {
            return new PopupResult<TModel>(model);
        }

#if SILVERLIGHT

        /// <summary>
        /// Shows the SaveFileDialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>The result.</returns>
        public static SaveFileDialogResult SaveFileDialog(SaveFileDialog dialog)
        {
            return new SaveFileDialogResult(dialog);
        }

        /// <summary>
        /// Shows the OpenFileDialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>The result.</returns>
        public static OpenFileDialogResult OpenFileDialog(OpenFileDialog dialog)
        {
            return new OpenFileDialogResult(dialog);
        }

#else

        /// <summary>
        /// Shows a CommonDialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>The result.</returns>
        public static ShowCommonDialogResult CommonDialog(CommonDialog dialog)
        {
            return new ShowCommonDialogResult(dialog);
        }

#endif

#if SILVERLIGHT_40

        /// <summary>
        /// Shows a toast notification for the specified duration in milliseconds.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel to show as a notification.</typeparam>
        /// <param name="durationInMilliseconds">The duration in milliseconds.</param>
        /// <returns>The result.</returns>
        public static NotificationResult<T> Notification<T>(int durationInMilliseconds)
        {
            return new NotificationResult<T>(durationInMilliseconds);
        }

        /// <summary>
        /// Shows a toast notification for the specified ViewModel for the specified duration in milliseconds.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="durationInMilliseconds">The duration in milliseconds.</param>
        /// <returns>The result.</returns>
        public static NotificationResult<T> Notification<T>(T viewModel, int durationInMilliseconds)
        {
            return new NotificationResult<T>(viewModel, durationInMilliseconds);
        }

#endif

        /// <summary>
        /// Marks the current ViewModel as busy.
        /// </summary>
        /// <returns>The result.</returns>
        public static BusyResult Busy()
        {
            return new BusyResult(true, null);
        }

        /// <summary>
        /// Marks the current ViewModel as busy, specifiying the ViewModel to use as the busy content.
        /// </summary>
        /// <param name="busyViewModel">The busy ViewModel.</param>
        /// <returns>The result.</returns>
        public static BusyResult Busy(object busyViewModel)
        {
            return new BusyResult(true, busyViewModel);
        }

        /// <summary>
        /// Marks the current ViewModel as not busy.
        /// </summary>
        /// <returns>The result.</returns>
        public static BusyResult NotBusy()
        {
            return new BusyResult(false, null);
        }

        /// <summary>
        /// Focuses the specified ViewModel.
        /// </summary>
        /// <param name="model">The ViewModel.</param>
        /// <returns>The result.</returns>
        public static FocusResult Focus(object model)
        {
            return new FocusResult(model, null);
        }

        /// <summary>
        /// Focuses the specified model's property.
        /// </summary>
        /// <param name="model">The ViewModel.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public static FocusResult Focus(object model, string property)
        {
            return new FocusResult(model, property);
        }

        /// <summary>
        /// Focuses the specified model's property.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel.</typeparam>
        /// <typeparam name="K">The type of the property.</typeparam>
        /// <param name="model">The ViewModel.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public static FocusResult Focus<T,K>(T model, Expression<Func<T,K>> property)
        {
            return new FocusResult(model, property.GetMemberInfo().Name);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <returns>The result.</returns>
        public static MessageBoxResult MessageBox(string text)
        {
            return new MessageBoxResult(text);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="caption">The caption text.</param>
        /// <returns>The result.</returns>
        public static MessageBoxResult MessageBox(string text, string caption)
        {
            return new MessageBoxResult(text, caption);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="caption">The caption text.</param>
        /// <param name="handleResult">The result callback.</param>
        /// <returns>The result.</returns>
        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult)
        {
            return new MessageBoxResult(text, caption, handleResult);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="caption">The caption text.</param>
        /// <param name="handleResult">The result callback.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <returns>The result.</returns>
        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
        {
            return new MessageBoxResult(text, caption, handleResult, possibleAnswers);
        }
    }
}