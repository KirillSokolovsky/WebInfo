namespace DomEventsSourceParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EventPropertyInfo
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string Description { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
