namespace WebInfo.Desktop.Models
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Models;
    using System.Windows;

    public class VersionedElementViewModel : ReactiveObject
    {
        public VersionedElementViewModel(WebElementViewModel source, List<string> existedNames)
        {
            Source = source;
            var info = WebFactory.CreateInfoFromModel(source);
            Updated = WebFactory.CreateModelFromInfo(info);
            ExistedNames = existedNames;
        }

        private WebElementViewModel _source;
        public WebElementViewModel Source
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref _source, value);
        }

        private WebElementViewModel _updated;
        public WebElementViewModel Updated
        {
            get => _updated;
            set => this.RaiseAndSetIfChanged(ref _updated, value);
        }

        public List<string> ExistedNames { get; set; }

        private bool _isUpdateMode;
        public bool IsUpdateMode
        {
            get => _isUpdateMode;
            set => this.RaiseAndSetIfChanged(ref _isUpdateMode, value);
        }

        public bool Verify()
        {
            var dict = new Dictionary<string, string>
            {
                { nameof(Updated.Name), Updated.Name?.Trim() },
                { nameof(Updated.Description), Updated.Description?.Trim() },
                { nameof(Updated.WebLocator.LocatorValue), Updated.WebLocator.LocatorValue?.Trim() }
            };

            var sb = new StringBuilder();
            foreach (var p in dict)
            {
                if (string.IsNullOrEmpty(p.Value) || (p.Value?.Contains(WebFactory.MockString) ?? false))
                    sb.AppendLine($"{p.Value} field is empty or has to be changed");
            }

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (ExistedNames != null)
            {
                if (ExistedNames.Contains(Updated.Name))
                {
                    MessageBox.Show($"Element with name {Updated.Name} already exists",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        public void MergeToSource()
        {
            Source.Name = Updated.Name.Trim();
            Source.Description = Updated.Description.Trim();
            Source.IsKey = Updated.IsKey;
            Source.WebLocator.IsRelative = Updated.WebLocator.IsRelative;
            Source.WebLocator.LocatorValue = Updated.WebLocator.LocatorValue.Trim();
            Source.WebLocator.LocatorType = Updated.WebLocator.LocatorType;
        }
    }
}
