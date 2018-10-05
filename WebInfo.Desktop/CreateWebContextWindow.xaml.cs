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

    public partial class CreateWebContextWindow : MetroWindow
    {
        public WebContext WebContext { get; set; }
        private readonly bool _isUpdateMode = false;
        private readonly WebContext _sourceObject;
        private readonly WebContext _sourceObjectCleaned;

        public CreateWebContextWindow(string title, WebContext webContext = null)
        {
            _isUpdateMode = webContext != null;

            if (_isUpdateMode)
            {
                _sourceObject = webContext;
                var curChildren = _sourceObject.WebElements;
                _sourceObject.WebElements = null;
                _sourceObjectCleaned = _sourceObject.GetCopy() as WebContext;
                _sourceObject.WebElements = curChildren;
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
                WebElementControl.SetWebElement(_sourceObjectCleaned);
                AllowedTagsControl.SetTags(_sourceObjectCleaned.AllowedTags);
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
            WebContext context;
            if (_isUpdateMode)
            {
                context = _sourceObject;
            }
            else
            {
                context = new WebContext();
            }

            var info = WebElementControl.GetWebElementInfo();
            context.AllowedTags = AllowedTagsControl.Tags;
            context.Name = info.Name;
            context.Description = info.Description;
            context.IsKeyElement = info.IsKeyElement;
            context.Tags = info.Tags;
            context.WebLocator = info.WebLocator.GetCopy();

            WebContext = context;

            DialogResult = true;
            Close();
        }
    }
}
