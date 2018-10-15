namespace DomEventsSourceParser
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EventsCategoryInfo
    {
        public string Name { get; set; }

        public List<EventInfo> Events { get; set; }


        public EventsCategoryInfo Parse(HtmlNode htmlNode)
        {
            var captionElement = htmlNode.SelectSingleNode(".//caption");
            if (captionElement != null)
            {
                Name = captionElement.InnerText;
            }
            else
            {
                var h3SpanElement = htmlNode.SelectSingleNode("(.//preceding-sibling::h3)[last()]");
                Name = h3SpanElement.InnerHtml;
            }

            return this;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
