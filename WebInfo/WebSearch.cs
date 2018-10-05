namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class WebSearch
    {
        public WebSearch ParentSearch { get; set; }
        public WebLocatorType LocatorType { get; set; }
        public string LocatorValue { get; set; }
    }
}
