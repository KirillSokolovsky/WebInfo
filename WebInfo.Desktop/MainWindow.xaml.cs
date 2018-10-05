namespace WebInfo.Desktop
{
    using WebInfo;
    using WebInfo.Desktop.Models;
    using MahApps.Metro.Controls;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
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

    public partial class MainWindow : MetroWindow
    {
        private TestsDataStorage _testsDataStorage;
        private WebInfoItemsStorage _webElementRepository;

        private ElementsViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Services.ElementsTreeView = ElementsTreeView;

            _viewModel = new ElementsViewModel();
            _viewModel.Elements = new ObservableCollection<WebElementViewModel>();

            DataContext = _viewModel;

            LoadInfos();
        }

        private void LoadInfos()
        {
            _testsDataStorage = new TestsDataStorage(@"..\..\..\..\EmailService.E2E.Tests.TestData");
            _testsDataStorage.Initialize();
            _webElementRepository = _testsDataStorage.GetWebInfoItemsStorage();

            foreach (var context in _webElementRepository.GetItems())
            {
                var model = WebFactory.CreateModelFromInfo(context);
                _viewModel.Elements.Add(model);
            }
        }

        private void ElementsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sel = ElementsTreeView.SelectedItem as TreeViewItem;
            if (sel != null) _viewModel.SelectedElement = null;
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchElementTextBox.Text = "";
        }

        private void AddContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var contextModel = WebFactory.CreateWebContextModel();
            var dialog = new WebElementDetailsWindow(contextModel, _viewModel.GetExistedNames(), false);
            if (dialog.ShowDialog() != true) return;

            _viewModel.Elements.Add(contextModel);
        }

        private void CopyNameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;
            Clipboard.SetText(_viewModel.SelectedElement.Name);
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;

            List<string> existedNames = null;
            if (_viewModel.SelectedElement is WebContextViewModel)
                existedNames = _viewModel.GetExistedNames();
            else
            {
                var parent = _viewModel.SelectedElement.Parent;
                if (parent == null)
                {
                    MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} doesn't have parent. But has to");
                    return;
                }
                if (parent is CombinedElementViewModel combined)
                {
                    existedNames = combined.GetExistedNames();
                }
                else
                {
                    MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} has parent that is not nested from CombinedElementViewModel. But has to");
                    return;
                }
            }

            var dialog = new WebElementDetailsWindow(_viewModel.SelectedElement, existedNames, true);
            if (dialog.ShowDialog() != true) return;
        }

        public bool CheckAndGetParent(out CombinedElementViewModel cparent)
        {
            cparent = null;
            if (_viewModel.SelectedElement is WebContextViewModel webContext)
            {
                cparent = null;
            }
            else
            {
                var parent = _viewModel.SelectedElement.Parent;
                if (parent == null)
                {
                    MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} doesn't have parent. But has to");
                    return false;
                }
                if (parent is CombinedElementViewModel combined)
                {
                    cparent = combined;
                }
                else
                {
                    MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} has parent that is not nested from CombinedElementViewModel. But has to");
                    return false;
                }
            }
            return true;
        }

        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;
            if (!CheckAndGetParent(out var cparent)) return;

            var result = MessageBox.Show($"Confirm removing of element: {_viewModel.SelectedElement.ElementType} | {_viewModel.SelectedElement.Name}",
                caption: "Removal confirmation",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.No);

            if (result != MessageBoxResult.Yes) return;

            if (cparent != null)
                cparent.Elements.Remove(_viewModel.SelectedElement);
            else _viewModel.Elements.Remove(_viewModel.SelectedElement);
            _viewModel.SelectedElement = null;
        }

        private void AddElementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;

            if (!(_viewModel.SelectedElement is CombinedElementViewModel combined))
            {
                MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} is not nested from CombinedElementViewModel. But has to");
                return;
            }

            var model = WebFactory.CreateWebElementModel();

            var dialog = new WebElementDetailsWindow(model, combined.GetExistedNames(), false);
            if (dialog.ShowDialog() != true) return;

            if (combined.Elements == null)
                combined.Elements = new ObservableCollection<WebElementViewModel>();
            model.Parent = combined;
            combined.Elements.Add(model);

            _viewModel.SelectedElement = model;
        }

        private void AddControlMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;

            if (!(_viewModel.SelectedElement is CombinedElementViewModel combined))
            {
                MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} is not nested from CombinedElementViewModel. But has to");
                return;
            }

            var model = WebFactory.CreateCombinedElementModel();

            var dialog = new WebElementDetailsWindow(model, combined.GetExistedNames(), false);
            if (dialog.ShowDialog() != true) return;

            if (combined.Elements == null)
                combined.Elements = new ObservableCollection<WebElementViewModel>();
            model.Parent = combined;
            combined.Elements.Add(model);

            _viewModel.SelectedElement = model;
        }

        private void AddDropDownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;

            if (!(_viewModel.SelectedElement is CombinedElementViewModel combined))
            {
                MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} is not nested from CombinedElementViewModel. But has to");
                return;
            }

            var model = WebFactory.CreateDropDownElementModel();

            var dialog = new WebElementDetailsWindow(model, combined.GetExistedNames(), false);
            if (dialog.ShowDialog() != true) return;

            var ddName = model.Name.Replace("DropDown", "").Trim();

            var input = model.Elements.First(el => el.InnerKey == DropDownElementInfo.Keys.Input);
            input.Name = $"{ddName} Input";

            dialog = new WebElementDetailsWindow(input, model.GetExistedNames(), true);
            if (dialog.ShowDialog() != true) return;

            var option = model.Elements.First(el => el.InnerKey == DropDownElementInfo.Keys.Option);
            option.Name = $"{ddName} Option";

            dialog = new WebElementDetailsWindow(option, model.GetExistedNames(), true);
            if (dialog.ShowDialog() != true) return;


            if (combined.Elements == null)
                combined.Elements = new ObservableCollection<WebElementViewModel>();
            model.Parent = combined;
            combined.Elements.Add(model);

            _viewModel.SelectedElement = model;
        }

        private void AddRadioGroupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;

            if (!(_viewModel.SelectedElement is CombinedElementViewModel combined))
            {
                MessageBox.Show($"Selected Element: {_viewModel.SelectedElement.Name} is not nested from CombinedElementViewModel. But has to");
                return;
            }

            var model = WebFactory.CreateRadioGroupModel();

            var dialog = new WebElementDetailsWindow(model, combined.GetExistedNames(), false);
            if (dialog.ShowDialog() != true) return;

            var rgName = model.Name.Replace("RadioGroup", "").Trim();
            var option = model.Elements.First(el => el.InnerKey == RadioGroupElementInfo.Keys.Option);
            option.Name = $"{rgName} Option";

            dialog = new WebElementDetailsWindow(option, model.GetExistedNames(), true);
            if (dialog.ShowDialog() != true) return;

            if (combined.Elements == null)
                combined.Elements = new ObservableCollection<WebElementViewModel>();
            model.Parent = combined;
            combined.Elements.Add(model);

            _viewModel.SelectedElement = model;
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Elements == null) return;

            var infos = new List<WebContext>();
            foreach (var model in _viewModel.Elements)
            {
                var info = WebFactory.CreateInfoFromModel(model)
                    as WebContext;

                if (info == null)
                    continue;

                infos.Add(info);
            }

            _webElementRepository.SetItems(infos);
            _webElementRepository.Save();
        }

        private void MoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;
            if (_viewModel.SelectedElement is WebContextViewModel) return;

            _viewModel.MovingItem = _viewModel.SelectedElement;
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedElement == null) return;
            if (_viewModel.MovingItem == null) return;

            if (!(_viewModel.SelectedElement is CombinedElementViewModel combined)) return;
            if (combined.ElementType != WebElementTypes.Context 
                && combined.ElementType != WebElementTypes.Control) return;
            if (combined.IsSelfOrChildren(_viewModel.MovingItem)) return;

            if (_viewModel.MovingItem.Parent == null) return;
            if (!(_viewModel.MovingItem.Parent is CombinedElementViewModel combinedParent)) return;
            if (combinedParent.ElementType != WebElementTypes.Context
                && combinedParent.ElementType != WebElementTypes.Control) return;

            combinedParent.Elements.Remove(_viewModel.MovingItem);
            _viewModel.MovingItem.Parent = combined;

            if (combined.Elements == null)
                combined.Elements = new ObservableCollection<WebElementViewModel>();
            combined.Elements.Add(_viewModel.MovingItem);
            _viewModel.MovingItem = null;
        }
    }
}
