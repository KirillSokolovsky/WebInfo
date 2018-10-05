namespace WebInfo
{
    public interface IWebElementsRepository
    {
        void LoadWebContexts();
        WebContext GetWebContext(string contextName);
    }
}