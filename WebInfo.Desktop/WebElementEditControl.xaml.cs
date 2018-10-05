namespace EmailService.E2E.Tests.Tools.WebInfoDesktop
{
    using EmailService.E2E.Tests.Modules.WebInfo;
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    public partial class WebElementEditControl : UserControl
    {
        public WebElementInfo WebElement { get; set; }

        public WebElementEditControl()
        {
            InitializeComponent();
        }

        public void SetWebElement(WebElementInfo webElement)
        {
            WebElement = webElement;

            NameTextBox.Text = webElement.Name;
            DescriptionTextBox.Text = webElement.Description;
            IsRelativeCheckBox.IsChecked = webElement.WebLocator.IsRelative;
            LocatorValueTextBox.Text = webElement.WebLocator.LocatorValue;
            IsKeyCheckbox.IsChecked = webElement.IsKeyElement;
            foreach (var item in LocatorTypeComboBox.Items)
            {
                if ((item as ComboBoxItem).Content.ToString() == webElement.WebLocator.LocatorType.ToString())
                {
                    LocatorTypeComboBox.SelectedItem = item;
                }
            }

            TagsControl.SetTags(webElement.Tags);
        }

        public WebElementInfo GetWebElementInfo()
        {
            if (WebElement == null)
            {
                WebElement = new WebElementInfo();
                WebElement.WebLocator = new WebLocatorInfo();
            }

            WebElement.Name = NameTextBox.Text;
            WebElement.Description = DescriptionTextBox.Text;
            WebElement.IsKeyElement = IsKeyCheckbox.IsChecked == true;
            WebElement.WebLocator.IsRelative = IsRelativeCheckBox.IsChecked == true;
            WebElement.WebLocator.LocatorValue = LocatorValueTextBox.Text;

            WebElement.WebLocator.LocatorType = (WebLocatorType)Enum.Parse(typeof(WebLocatorType), (LocatorTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString());

            return WebElement;
        }
    }
}
