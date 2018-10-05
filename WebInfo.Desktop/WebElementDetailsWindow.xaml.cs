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
    using Models;

    public partial class WebElementDetailsWindow : MetroWindow
    {
        public VersionedElementViewModel Model { get; set; }

        public WebElementDetailsWindow(WebElementViewModel model, List<string> existedNames, bool isUpdateMode)
        {
            if (isUpdateMode && existedNames != null)
                existedNames.Remove(model.Name);

            Model = new VersionedElementViewModel(model, existedNames);
            Model.IsUpdateMode = isUpdateMode;


            DataContext = Model;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Model.Verify()) return;

            Model.MergeToSource();

            DialogResult = true;
            Close();
        }
    }
}
