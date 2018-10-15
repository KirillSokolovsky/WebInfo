using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace DomEventsSourceParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var reDownload = true;
            var savedHtmlPath = "events.html";

            if (!File.Exists(savedHtmlPath) || reDownload)
            {
                var cl = new RestClient("https://developer.mozilla.org");
                var req = new RestRequest("/en-US/docs/Web/Events");
                var resp = cl.Get(req);
                File.WriteAllText(savedHtmlPath, resp.Content);
            }
            var html = File.ReadAllText(savedHtmlPath);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var eventGroupsNodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='standard-table']");

            var eventGroups = eventGroupsNodes.Select(eg => new EventsCategoryInfo().Parse(eg))
                .ToList();
        }
    }
}
