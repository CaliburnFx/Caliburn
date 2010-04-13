namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq.Expressions;
    using Core;
    using Microsoft.Win32;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;
    using Questions;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public static class Show
    {
        private static IViewModelFactory _viewModelFactory;

        public static void Initialize(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public static OpenScreenSubjectResult ChildFor(IScreenSubject screenSubject)
        {
            return new OpenScreenSubjectResult(screenSubject);
        }

        public static OpenScreenSubjectResult ChildFor<T>(T subject)
        {
            var screenSubject = new ScreenSubject<T>(subject);
            return new OpenScreenSubjectResult(screenSubject);
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

#if !WP7
        public static OpenDialogResult<IScreen<T>> DialogFor<T>(T subject)
        {
            var screen = _viewModelFactory.CreateFor(subject);
            return new OpenDialogResult<IScreen<T>>(screen);
        }

        public static OpenDialogResult<TModal> Dialog<TModal>()
            where TModal : IScreen
        {
            return new OpenDialogResult<TModal>();
        }

        public static OpenDialogResult<TModal> Dialog<TModal>(TModal modal)
            where TModal : IScreen
        {
            return new OpenDialogResult<TModal>(modal);
        }
#endif

        public static PopupResult<TPopup> Popup<TPopup>()
        {
            return new PopupResult<TPopup>();
        }

        public static PopupResult<TPopup> Popup<TPopup>(TPopup popup)
        {
            return new PopupResult<TPopup>(popup);
        }

#if SILVERLIGHT

        public static SaveFileDialogResult Dialog(SaveFileDialog dialog)
        {
            return new SaveFileDialogResult(dialog);
        }

        public static OpenFileDialogResult Dialog(OpenFileDialog dialog)
        {
            return new OpenFileDialogResult(dialog);
        }

#else

        public static ShowCommonDialogResult Dialog(CommonDialog dialog)
        {
            return new ShowCommonDialogResult(dialog);
        }

#endif

#if SILVERLIGHT_40 && !WP7

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

#if !WP7
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
#endif
    }
}