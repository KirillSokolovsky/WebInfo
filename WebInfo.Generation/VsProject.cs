namespace WebInfo.Generation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class VsProject
    {
        public string Name { get; set; }
        private string _fullPath { get; set; }
        public DateTime LastWriteTime { get; set; }

        public VsProject(string fullPath)
        {
            _fullPath = fullPath;
            Name = Path.GetFileNameWithoutExtension(fullPath);
        }

        private string _rememberedSourceLineWithXmlNamespace;
        public void Scan()
        {
            var lines = File.ReadAllLines(_fullPath);
            _rememberedSourceLineWithXmlNamespace = lines[1];
            lines[1] = "<Project>";
            var xml = XElement.Parse(string.Join(Environment.NewLine, lines));
            
        }
    }
}
