using WebInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Con = System.Console;

namespace WebInfo.Generation.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var webInfo = GetTestInfo();

            var converter = new WebInfoToCodeConverter();
            var genData = converter.GetCompilationUnitForContext(webInfo);

            var text = genData.NormalizeWhitespace().ToFullString();

            var csFilePath = @"..\..\info.cs";
            var fi = new FileInfo(csFilePath);
            File.WriteAllText(csFilePath, text);

            //var q = new _Login_Form().Info.Locator.LocatorValue;

            Con.WriteLine(text);
            Con.ReadLine();
        }


        public static WebContext GetTestInfo()
        {
            var emailInput = new WebElementInfo
            {
                Name = "Email Input",
                Description = "Input for Email",
                IsKey = false,
                InnerKey = null,
                Locator = new WebLocatorInfo
                {
                    LocatorType = WebLocatorType.XPath,
                    LocatorValue = "//input[@id='email']",
                    IsRelative = false
                }
            };

            var loginForm = new WebContext
            {
                Name = "Login Form",
                Description = "Form to do login",
                IsKey = false,
                InnerKey = null,
                Elements = new List<WebElementInfo>(),
                Locator = new WebLocatorInfo
                {
                    LocatorType = WebLocatorType.XPath,
                    LocatorValue = ".",
                    IsRelative = false
                }
            };
            loginForm.Elements.Add(emailInput);

            return loginForm;
        }
        public static void T1(int a, int b)
        {
        }
    }
}


namespace Test
{
    public static partial class ConsumerSite
    {
        public static _Login_Form Login_Form { get; set; } = new _Login_Form();

        /// <summary>
        /// Login Form Desc
        /// </summary>
        public class _Login_Form : CombinedWebElementInfo
        {
            public _Info.Info Info { get; set; } = new _Info.Info();

            public static class _Info
            {
                public const string ElementType = "WebContext";
                public const string Name = "Login Form";
                public const string Description = "Login Form Desc";
                public const bool IsKey = false;
                public const string InnerKey = null;

                public static class _Locator
                {
                    public const WebLocatorType LocatorType = WebLocatorType.XPath;
                    public const string LocatorValue = "//form";
                    public const bool IsRelative = false;
                }

                public class Info
                {
                    /// <summary>
                    /// WebLocatorType.XPath
                    /// </summary>
                    public string ElementType;
                    /// <summary>
                    /// Login Form
                    /// </summary>
                    public string Name;
                    /// <summary>
                    /// Login Form Desc
                    /// </summary>
                    public string Description;
                    public bool IsKey;
                    public string InnerKey;

                    public Locator Locator;
                }

                public class Locator
                {
                    /// <summary>
                    /// //form
                    /// </summary>
                    public string LocatorValue;
                }
            }

            public _Email_Input Email_Input { get; set; }

            /// <summary>
            /// Email Desc
            /// </summary>
            public class _Email_Input : WebElementInfo
            {
                public static class _Info
                {
                    public const string Name = "Email Input";
                    public const string Description = "Email Input Desc";

                    public static class Locator
                    {
                        public const WebLocatorType LocatorType = WebLocatorType.XPath;
                        public const string LocatorValue = ".//input";
                        public const bool IsRelative = true;
                    }
                }
            }

            public _Login_Form()
            {
                Elements = new List<WebElementInfo>();
                Email_Input = new _Email_Input();
                Email_Input.Parent = this;
                Elements.Add(Email_Input);
            }
        }
    }

}
