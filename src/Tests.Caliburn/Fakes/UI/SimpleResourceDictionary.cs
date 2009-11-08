using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tests.Caliburn.Fakes.UI
{
    public class SimpleResourceDictionary : ResourceDictionary
    {
        public static readonly string TextBlockStyleKey = "textBlockStyle";

        public SimpleResourceDictionary()
        {
            Add(TextBlockStyleKey, CreateStyle());
        }

        private Style CreateStyle()
        {
            var style = new Style {TargetType = typeof(TextBlock)};

            style.Setters.Add(
                new Setter 
                { 
                    Property = TextBlock.ForegroundProperty,
                    Value = Brushes.AliceBlue
                });

            return style;
        }
    }
}