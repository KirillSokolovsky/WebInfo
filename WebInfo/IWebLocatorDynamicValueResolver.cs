namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWebLocatorDynamicValueResolver
    {
        string ResolveDynamicLocatorValue(string locator, params (string parName, object parValue)[] values);
    }
}
