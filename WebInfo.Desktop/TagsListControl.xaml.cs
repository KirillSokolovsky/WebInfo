namespace WebInfo.Desktop
{
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

    public partial class TagsListControl : UserControl
    {
        public List<string> Tags { get; set; }

        public TagsListControl()
        {
            InitializeComponent();
        }

        public void SetTags(List<string> tags)
        {
            Tags = tags?.ToList();
            if (Tags == null) return;

            foreach (var t in Tags)
            {
                TagsListBox.Items.Add(t);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var textDialog = new TextEditDialog("Create new Tag:", Tags);
            if (textDialog.ShowDialog() == true)
            {
                if (Tags == null) Tags = new List<string>();
                Tags.Add(textDialog.Text);
                TagsListBox.Items.Add(textDialog.Text);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTag == null) return;

            var textDialog = new TextEditDialog("Edit Tag:", Tags, _selectedTag);
            if (textDialog.ShowDialog() == true)
            {
                Tags.Remove(_selectedTag);
                TagsListBox.Items.Remove(_selectedTag);

                Tags.Add(textDialog.Text);
                TagsListBox.Items.Add(textDialog.Text);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTag == null) return;

            Tags.Remove(_selectedTag);
            TagsListBox.Items.Remove(_selectedTag);
        }

        private string _selectedTag = null;
        private void TagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TagsListBox.SelectedIndex > -1)
                _selectedTag = TagsListBox.SelectedItem as string;
            else
                _selectedTag = null;
        }
    }
}
