namespace WebInfo.Repository
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebElementsRepository : IWebElementsRepository
    {
        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        private List<FileInfo> _files;
        private DirectoryInfo _directory;
        private string _ext;

        private List<WebContext> _webContexts;


        public WebElementsRepository(string directoryPath, string ext = ".webinfo.json")
        {
            _ext = ext;
            _directory = new DirectoryInfo(directoryPath);
            _webContexts = new List<WebContext>();
        }

        public void AddWebContext(WebContext context)
        {
            if (_webContexts.Any(c => c.Name == context.Name))
                throw new Exception($"WebContext with name: {context.Name} already exists");

            _webContexts.Add(context);
        }

        public void DeleteWebContext(string name)
        {
            var toRemove = _webContexts.FirstOrDefault(c => c.Name == name)
                ?? throw new Exception($"There is no WebContext with name: {name}");

            _webContexts.Remove(toRemove);
        }

        public WebContext GetWebContextOrDefault(string contextName)
        {
            return _webContexts.FirstOrDefault(c => c.Name == contextName);
        }

        public List<WebContext> GetWebContexts()
        {
            return _webContexts.ToList();
        }

        public void LoadWebContexts()
        {
            if (!_directory.Exists)
                _directory.Create();

            _files = _directory.GetFiles($"*{_ext}")
                .ToList();

            foreach (var file in _files)
            {
                var json = File.ReadAllText(file.FullName);
                var context = JsonConvert.DeserializeObject<WebContext>(json, DefaultSerializerSettings);
                _webContexts.Add(context);
            }
        }

        public void SaveWebContexts()
        {
            var writtenFiles = new List<string>();

            foreach (var context in _webContexts)
            {
                var json = JsonConvert.SerializeObject(context, DefaultSerializerSettings);
                var fileName = $"{context.Name}{_ext}";
                var filePath = $"{_directory.FullName}\\{fileName}";
                File.WriteAllText(filePath, json);
                writtenFiles.Add(fileName);
            }

            _files = _directory.GetFiles($"*{_ext}")
                .ToList();
            var toRemove = _files.Where(f => !writtenFiles.Contains(f.Name))
                .ToList();

            toRemove.ForEach(f => f.Delete());
        }

        public void SetWebContexts(List<WebContext> webContexts)
        {
            _webContexts = webContexts.ToList();
        }
    }
}
