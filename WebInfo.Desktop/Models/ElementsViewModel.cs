namespace WebInfo.Desktop.Models
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ElementsViewModel : ReactiveObject
    {
        public ElementsViewModel()
        {
            this.WhenAnyValue(e => e.FilterText)
                .Subscribe(text => Filter(text));
        }

        private ObservableCollection<WebElementViewModel> _elements;
        public ObservableCollection<WebElementViewModel> Elements
        {
            get => _elements;
            set => this.RaiseAndSetIfChanged(ref _elements, value);
        }

        private WebElementViewModel _selectedElement;
        public WebElementViewModel SelectedElement
        {
            get => _selectedElement;
            set => this.RaiseAndSetIfChanged(ref _selectedElement, value);
        }

        private WebElementViewModel _movingItem;
        public WebElementViewModel MovingItem
        {
            get => _movingItem;
            set => this.RaiseAndSetIfChanged(ref _movingItem, value);
        }

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }

        public void Filter(string text)
        {
            if (_elements == null) return;
            foreach (var element in _elements)
            {
                element.Filter(text);
            }
        }

        public List<string> GetExistedNames()
        {
            return Elements?.Select(e => e.Name).ToList();
        }
    }
}
