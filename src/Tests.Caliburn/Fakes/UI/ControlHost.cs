namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;

    public class ControlHost : UserControl
    {
        readonly TextBox param2;
        readonly NameScope scope;
        public TextBox MethodResult;
        public TextBox Param1;

        public ControlHost()
        {
            var stack = new StackPanel();
            Content = stack;

            Param1 = new TextBox {
                Name = "param1"
            };
            param2 = new TextBox {
                Name = "param2"
            };

            stack.Children.Add(Param1);
            stack.Children.Add(param2);

            scope = new NameScope();

            NameScope.SetNameScope(this, scope);

            scope.RegisterName("param1", Param1);
            scope.RegisterName("param2", param2);

            MethodResult = new TextBox {
                Name = "MethodResult"
            };
            scope.RegisterName("MethodResult", MethodResult);
        }

        public void SetParam1(object param1)
        {
            Param1.Text = param1.ToString();
        }

        public void SetParam2(object param2)
        {
            this.param2.Text = param2.ToString();
        }
    }
}