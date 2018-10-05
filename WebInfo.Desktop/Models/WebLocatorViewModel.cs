namespace WebInfo.Desktop.Models
{
    using WebInfo;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebLocatorViewModel : ReactiveObject
    {
        private bool _isRelative;
        public bool IsRelative
        {
            get => _isRelative;
            set => this.RaiseAndSetIfChanged(ref _isRelative, value);
        }

        private string _locatorValue;
        public string LocatorValue
        {
            get => _locatorValue;
            set => this.RaiseAndSetIfChanged(ref _locatorValue, value);
        }

        private WebLocatorType _locatorType;
        public WebLocatorType LocatorType
        {
            get => _locatorType;
            set => this.RaiseAndSetIfChanged(ref _locatorType, value);
        }

        private static List<WebLocatorType> _locatorTypes;

        public List<WebLocatorType> LocatorTypes
        {
            get
            {
                if (_locatorTypes == null)
                {
                    _locatorTypes = Enum.GetValues(typeof(WebLocatorType))
                        .Cast<WebLocatorType>()
                        .ToList();
                }
                return _locatorTypes;
            }
        }

        public void FillFromInfo(WebLocatorInfo info)
        {
            LocatorType = info.LocatorType;
            LocatorValue = info.LocatorValue;
            IsRelative = info.IsRelative;
        }

        public void FillInfo(WebLocatorInfo info)
        {
            info.LocatorValue = LocatorValue;
            info.LocatorType = LocatorType;
            info.IsRelative = IsRelative;
        }
    }
}
