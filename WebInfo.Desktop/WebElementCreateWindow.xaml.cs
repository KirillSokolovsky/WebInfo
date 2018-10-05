namespace EmailService.E2E.Tests.Tools.WebInfoDesktop
{
    using EmailService.E2E.Tests.Modules.WebInfo;
    using MahApps.Metro.Controls;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public partial class WebElementCreateWindow : MetroWindow
    {
        public WebElementInfo WebElement { get; set; }
        private readonly WebElementInfo _sourceObject;
        private readonly WebElementInfo _clearedObject;
        private readonly bool _isUpdateMode = false;


        public WebElementCreateWindow(string title, WebElementInfo webElement = null)
        {
            _isUpdateMode = webElement != null;
            if (_isUpdateMode)
            {
                _sourceObject = webElement;
                _clearedObject = _sourceObject.GetCopy();
            }

            InitializeComponent();

            Title = title;

            Render();
        }

        private void Render()
        {
            if (_isUpdateMode)
            {
                AcceptButton.Content = "Update";
                WebElementControl.SetWebElement(_clearedObject);
            }
            else
            {
                AcceptButton.Content = "Create";
            }
        }

        private void CancelButtom_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            WebElementInfo webElement;
            if (_isUpdateMode)
            {
                webElement = _sourceObject;
            }
            else
            {
                webElement = new WebElementInfo();
            }

            var info = WebElementControl.GetWebElementInfo();
            webElement.Name = info.Name;
            webElement.Description = info.Description;
            webElement.IsKeyElement = info.IsKeyElement;
            webElement.Tags = info.Tags;
            webElement.WebLocator = info.WebLocator.GetCopy();

            WebElement = webElement;

            DialogResult = true;
            Close();
        }
    }
}
