using System.Windows;
using System.Windows.Controls;

namespace Tests.Caliburn.Fakes.UI
{
    public class ControlHost : UserControl
    {
        public TextBox _param1;
        private TextBox _param2;
        public TextBox _methodResult;
        private NameScope _scope;

        public ControlHost()
        {
            var stack = new StackPanel();
            Content = stack;

            _param1 = new TextBox {Name = "param1"};
            _param2 = new TextBox {Name = "param2"};

            stack.Children.Add(_param1);
            stack.Children.Add(_param2);

            _scope = new NameScope();

            NameScope.SetNameScope(this, _scope);

            _scope.RegisterName("param1", _param1);
            _scope.RegisterName("param2", _param2);

            _methodResult = new TextBox {Name = "MethodResult"};
            _scope.RegisterName("MethodResult", _methodResult);
        }

        public void SetParam1(object param1)
        {
            _param1.Text = param1.ToString();
        }

        public void SetParam2(object param2)
        {
            _param2.Text = param2.ToString();
        }
    }
}