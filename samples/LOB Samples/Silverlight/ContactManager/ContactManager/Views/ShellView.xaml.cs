namespace ContactManager.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public partial class ShellView : UserControl
    {
        public ShellView()
        {
            InitializeComponent();

            Application.Current.Host.Content.Resized += Content_Resized;
            Application.Current.Host.Content.FullScreenChanged += Content_Resized;

            Resize();
        }

        private void Content_Resized(object sender, EventArgs e)
        {
            Resize();
        }

        private void Resize()
        {
            var originalWidth = 750;
            var originalHeight = 600;

            double currentWidth = Application.Current.Host.Content.ActualWidth;
            double currentHeight = Application.Current.Host.Content.ActualHeight;

            double uniformScaleAmount = Math.Min((currentWidth / originalWidth), (currentHeight / originalHeight));
            rootScaleTransform.ScaleX = uniformScaleAmount;
            rootScaleTransform.ScaleY = uniformScaleAmount;

            double scaledWidth = originalWidth * uniformScaleAmount;
            double scaledHeight = originalHeight * uniformScaleAmount;
            rootTranslateTransform.X = (currentWidth - scaledWidth) / 2d;
            rootTranslateTransform.Y = (currentHeight - scaledHeight) / 2d;
        }
    }
}