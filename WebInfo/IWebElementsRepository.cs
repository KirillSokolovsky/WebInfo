namespace WebInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWebElementsRepository : IWebElementsReadOnlyRepository
    {
        void SaveWebContexts();

        void AddWebContext(WebContext context);
        void DeleteWebContext(string name);
        void SetWebContexts(List<WebContext> webContexts);
    }
}
