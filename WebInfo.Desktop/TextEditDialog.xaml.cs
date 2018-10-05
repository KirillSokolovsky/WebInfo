namespace WebInfo.Desktop
{
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

    public partial class TextEditDialog : MetroWindow
    {
        private readonly string _title;
        private string _text;
        public string Text { get; set; }

        private readonly bool _isUpdatedMode = false;
        private readonly List<string> _notPermittedValues;

        public TextEditDialog(string title, List<string> notPermittedValues = null, string text = null)
        {
            Text = text;
            _title = title;
            _isUpdatedMode = text != null;
            _notPermittedValues = notPermittedValues?.ToList() ?? new List<string>();

            if (_isUpdatedMode && _notPermittedValues != null && _notPermittedValues.Contains(text))
                _notPermittedValues.Remove(text);

            InitializeComponent();

            Render();
        }

        private void Render()
        {
            Title = _title;

            AcceptButton.Content = _isUpdatedMode
                ? "Update"
                : "Create";

            AcceptButton.IsEnabled = false;

            if (_isUpdatedMode)
            {
                TextTextBox.Text = Text;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Text = _text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = TextTextBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                AcceptButton.IsEnabled = false;
                WarningLabel.Content = "Value shouldn't be empty";
                return;
            }

            if (_notPermittedValues.Contains(text))
            {
                AcceptButton.IsEnabled = false;
                WarningLabel.Content = $"Value '{text}' already exists";
                return;
            }

            if (_isUpdatedMode && text == Text)
            {
                AcceptButton.IsEnabled = false;
                WarningLabel.Content = $"Value '{text}' is the same as edited";
                return;
            }

            _text = text;
            AcceptButton.IsEnabled = true;
        }
    }
}
