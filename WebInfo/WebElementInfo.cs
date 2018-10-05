namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebElementInfo
    {
        public WebElementInfo()
        {
            ElementType = WebElementTypes.Element;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ElementType { get; set; }
        public string InnerKey { get; set; }
        public bool IsKey { get; set; }


        public WebLocatorInfo Locator { get; set; }
        public List<string> Tags { get; set; }

        public CombinedWebElementInfo Parent { get; set; }

        private WebSearch _webSearch;
        public WebSearch GetWebSearch()
        {
            if (_webSearch == null)
                _webSearch = BuildWebSearch();
            return _webSearch;
        }

        public WebElementInfo GetCopyWithResolvedLocator(params string[] values)
        {
            var copy = GetCopyWithoutParent();
            copy.Parent = Parent;

            var locator = Locator.LocatorValue;

            var parts = locator.Split(new[] { "'$", "$'" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];

                var partIndex = i * 2 + 1;

                if (partIndex > parts.Length)
                    throw new Exception("Error occurred during resolving locator." +
                        $"{this}" +
                        $"Locator: {locator}" +
                        $"Values: {string.Join(", ", values)}");

                var part = parts[partIndex];
                part = part.Replace($"\"{{{i}}}\"", $"'{value}'");
                parts[partIndex] = part;
            }

            locator = string.Join("", parts);

            copy.Locator.LocatorValue = locator;
            return copy;
        }

        private WebSearch BuildWebSearch()
        {
            if (!Locator.IsRelative || Parent == null)
            {
                return new WebSearch
                {
                    ParentSearch = null,
                    LocatorType = Locator.LocatorType,
                    LocatorValue = Locator.LocatorValue
                };
            }

            var currentSearch = new WebSearch
            {
                LocatorType = Locator.LocatorType,
                LocatorValue = Locator.LocatorValue
            };
            var parentSearch = Parent.GetWebSearch();

            if (parentSearch.LocatorType == currentSearch.LocatorType
                && (currentSearch.LocatorType == WebLocatorType.Css
                 || currentSearch.LocatorType == WebLocatorType.XPath))
            {
                if (currentSearch.LocatorType == WebLocatorType.XPath)
                {
                    currentSearch.LocatorValue = MergeXPath(parentSearch.LocatorValue, currentSearch.LocatorValue);
                }
                else
                {
                    currentSearch.LocatorValue = MergeCss(parentSearch.LocatorValue, currentSearch.LocatorValue);
                }
                currentSearch.ParentSearch = parentSearch.ParentSearch;
            }
            else
            {
                currentSearch.ParentSearch = parentSearch;
            }

            return currentSearch;
        }
        public string MergeXPath(string what, string with)
        {
            var xpath = "";

            var count = -1;
            while (with[++count] == '(') ;

            var prefix = "";
            if (count > 0)
                prefix = new string('(', count);

            if (with[count] == '.')
                count++;
            var postfix = with.Substring(count);

            xpath = prefix + what + postfix;

            return xpath;
        }
        private string MergeCss(string what, string with)
        {
            return $"{what} {with}";
        }

        public virtual WebElementInfo GetCopyWithoutParent(WebElementInfo webElementInfo = null)
        {
            if (webElementInfo == null)
                webElementInfo = new WebElementInfo();

            webElementInfo.Name = Name;
            webElementInfo.Description = Description;
            webElementInfo.ElementType = ElementType;
            webElementInfo.InnerKey = InnerKey;
            webElementInfo.IsKey = IsKey;
            webElementInfo.Locator = Locator?.GetCopy();
            webElementInfo.Tags = Tags?.ToList();

            return webElementInfo;
        }

        private StringBuilder _sb;
        public override string ToString()
        {
            if (_sb == null)
            {
                _sb = new StringBuilder();
                _sb.Append(ElementType);
                _sb.Append(" ");
                _sb.Append(Name);

                if (Parent != null)
                {
                    _sb.Append(" on ");
                    _sb.Append(Parent.ElementType);
                    _sb.Append(" ");
                    _sb.Append(Parent.Name);
                }
            }

            return _sb.ToString();
        }
    }
}
