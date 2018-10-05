namespace WebInfo.Desktop.Models
{
    using WebInfo;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebElementViewModel : ReactiveObject
    {
        protected WebElementInfo _webElementInfo;

        public WebElementViewModel()
        {

        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        private string _elementType;
        public string ElementType
        {
            get => _elementType;
            set => this.RaiseAndSetIfChanged(ref _elementType, value);
        }

        private string _innerKey;
        public string InnerKey
        {
            get => _innerKey;
            set => this.RaiseAndSetIfChanged(ref _innerKey, value);
        }

        private bool _isKey;
        public bool IsKey
        {
            get => _isKey;
            set => this.RaiseAndSetIfChanged(ref _isKey, value);
        }

        private ObservableCollection<string> _tags;
        public ObservableCollection<string> Tags
        {
            get => _tags;
            set => this.RaiseAndSetIfChanged(ref _tags, value);
        }

        private WebLocatorViewModel _webLocator;
        public WebLocatorViewModel WebLocator
        {
            get => _webLocator;
            set => this.RaiseAndSetIfChanged(ref _webLocator, value);
        }

        private bool _isShown = true;
        public bool IsShown
        {
            get => _isShown;
            set => this.RaiseAndSetIfChanged(ref _isShown, value);
        }

        public WebElementViewModel Parent { get; set; }

        public virtual bool Filter(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Show();
                return true;
            }

            if (Name?.Contains(text) ?? false)
            {
                Show();
                return true;
            }

            Hide();
            return false;
        }

        public void Show()
        {
            IsShown = true;
        }

        public void Hide()
        {
            IsShown = false;
        }

        public virtual void FillFromInfo(WebElementInfo info)
        {
            Name = info.Name;
            Description = info.Description;
            ElementType = info.ElementType;
            InnerKey = info.InnerKey;
            IsKey = info.IsKey;
            if (info.Tags != null)
            {
                Tags = new ObservableCollection<string>();
                foreach (var tag in info.Tags)
                    Tags.Add(tag);
            }
            if (WebLocator == null)
                WebLocator = new WebLocatorViewModel();
            WebLocator.FillFromInfo(info.Locator);
        }

        public virtual void FillInfo(WebElementInfo info)
        {
            info.Name = Name;
            info.Description = Description;
            info.InnerKey = InnerKey;
            info.IsKey = IsKey;
            info.Tags = Tags?.ToList();
            if (info.Locator == null)
                info.Locator = new WebLocatorInfo();
            WebLocator.FillInfo(info.Locator);
        }
    }
}
