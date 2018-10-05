namespace WebInfo.Generation
{
    using WebInfo;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WebInfoGenerationData
    {
        public WebElementInfo Element { get; set; }
        public ClassDeclarationSyntax ClassSyntax { get; set; }
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
    }
}
