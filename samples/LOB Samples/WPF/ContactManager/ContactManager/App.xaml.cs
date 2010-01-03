namespace ContactManager
{
    using System.Collections.Generic;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.WPF.ApplicationFramework;
    using Presenters.Interfaces;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : CaliburnApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override object CreateRootModel()
        {
            return Container.GetInstance<IShellPresenter>();
        }

        protected override void ExecuteShutdownModel(ISubordinate model, System.Action completed)
        {
            var shell = Container.GetInstance<IShellPresenter>();
            var dialogPresenter = Container.GetInstance<IQuestionPresenter>();
            var questions = AssembleQuestions(model);

            dialogPresenter.Setup(questions, completed);
            shell.ShowDialog(dialogPresenter);
        }

        private IEnumerable<Question> AssembleQuestions(ISubordinate model)
        {
            var composite = model as ISubordinateComposite;

            if(composite != null)
            {
                foreach(var child in composite.GetChildren())
                {
                    foreach(var item in AssembleQuestions(child))
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                yield return (Question)model;
            }
        }
    }
}