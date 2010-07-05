namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq.Expressions;
    using Core;
    using Microsoft.Win32;
    using PresentationFramework.Screens;
    using Questions;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public static class Show
    {
        public static OpenScreenSubjectResult ChildSubject(IScreenSubject screenSubject)
        {
            return new OpenScreenSubjectResult(screenSubject);
        }

        public static OpenScreenSubjectResult ChildFor<T>(T subject)
        {
            return ChildSubject(new ScreenSubject<T>(subject));
        }

        public static OpenChildResult<TChild> Child<TChild>()
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>();
        }

        public static OpenChildResult<TChild> Child<TChild>(TChild child)
            where TChild : IScreen
        {
            return new OpenChildResult<TChild>(child);
        }

        public static DialogScreenSubjectResult DialogSubject(IScreenSubject screenSubject)
        {
            return new DialogScreenSubjectResult(screenSubject);
        }

        public static DialogScreenSubjectResult DialogFor<T>(T subject)
        {
            return new DialogScreenSubjectResult(new ScreenSubject<T>(subject));
        }

        public static OpenDialogResult<TModel> Dialog<TModel>()
            where TModel : IScreen
        {
            return new OpenDialogResult<TModel>();
        }

        public static OpenDialogResult<TModel> Dialog<TModel>(TModel model)
            where TModel : IScreen
        {
            return new OpenDialogResult<TModel>(model);
        }

        public static PopupResult<TModel> Popup<TModel>()
        {
            return new PopupResult<TModel>();
        }

        public static PopupResult<TModel> Popup<TModel>(TModel model)
        {
            return new PopupResult<TModel>(model);
        }

#if SILVERLIGHT

        public static SaveFileDialogResult SaveFileDialog(SaveFileDialog dialog)
        {
            return new SaveFileDialogResult(dialog);
        }

        public static OpenFileDialogResult OpenFileDialog(OpenFileDialog dialog)
        {
            return new OpenFileDialogResult(dialog);
        }

#else

        public static ShowCommonDialogResult CommonDialog(CommonDialog dialog)
        {
            return new ShowCommonDialogResult(dialog);
        }

#endif

#if SILVERLIGHT_40

        public static NotificationResult<T> Notification<T>(int durationInMilliseconds)
        {
            return new NotificationResult<T>(durationInMilliseconds);
        }

        public static NotificationResult<T> Notification<T>(T viewModel, int durationInMilliseconds)
        {
            return new NotificationResult<T>(viewModel, durationInMilliseconds);
        }

#endif

        public static BusyResult Busy()
        {
            return new BusyResult(true, null);
        }

        public static BusyResult Busy(object busyViewModel)
        {
            return new BusyResult(true, busyViewModel);
        }

        public static BusyResult NotBusy()
        {
            return new BusyResult(false, null);
        }

        public static FocusResult Focus(object model)
        {
            return new FocusResult(model, null);
        }

        public static FocusResult Focus(object model, string property)
        {
            return new FocusResult(model, property);
        }

        public static FocusResult Focus<T,K>(T model, Expression<Func<T,K>> property)
        {
            return new FocusResult(model, property.GetMemberInfo().Name);
        }

        public static MessageBoxResult MessageBox(string text)
        {
            return new MessageBoxResult(text);
        }

        public static MessageBoxResult MessageBox(string text, string caption)
        {
            return new MessageBoxResult(text, caption);
        }

        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult)
        {
            return new MessageBoxResult(text, caption, handleResult);
        }

        public static MessageBoxResult MessageBox(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
        {
            return new MessageBoxResult(text, caption, handleResult, possibleAnswers);
        }
    }
}