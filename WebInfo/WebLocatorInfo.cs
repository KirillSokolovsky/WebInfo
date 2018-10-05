namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class WebLocatorInfo
    {
        public string LocatorValue { get; set; }
        public WebLocatorType LocatorType { get; set; }
        public bool IsRelative { get; set; }

        public WebLocatorInfo GetCopy()
        {
            return new WebLocatorInfo
            {
                IsRelative = IsRelative,
                LocatorType = LocatorType,
                LocatorValue = LocatorValue
            };
        }

        public WebLocatorInfo GetResolvedDynamicLocator(params (string parName, object parValue)[] values)
        {
            return GetResolvedDynamicLocator(values);
        }

        private static IWebLocatorDynamicValueResolver _defaultResolver = new WebLocatorDefaultDynamicValuerResolver();
        public WebLocatorInfo GetResolvedDynamicLocator((string parName, object parValue)[] values, IWebLocatorDynamicValueResolver resolver = null )
        {
            resolver = resolver ?? _defaultResolver;

            var copiedLocator = GetCopy();

            copiedLocator.LocatorValue = resolver.ResolveDynamicLocatorValue(copiedLocator.LocatorValue, values);

            return copiedLocator;
        }
    }
}
