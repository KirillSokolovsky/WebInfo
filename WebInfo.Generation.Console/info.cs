using EmailService.E2E.Tests.Modules.WebInfo;
using System.Collections.Generic;

///<summary>
/// Form to do login
///</summary>
public class _Login_Form : WebContext
{
    ///<summary>
    /// Information about element
    ///</summary>
    public _Info.Info Info
    {
        get;
        set;
    }

    ///<summary>
    /// Get new element instance
    ///</summary>
    public static _Login_Form GetInstance(CombinedWebElementInfo parentElement) => new _Login_Form{Parent = parentElement};
    public static class _Info
    {
        public const string ElementType = "Context";
        public const string Name = "Login Form";
        public const string Description = "Form to do login";
        public const bool IsKey = false;
        public const string InnerKey = null;
        public static class _Locator
        {
            public const WebLocatorType LocatorType = WebLocatorType.XPath;
            public const string LocatorValue = ".";
            public const bool IsRelative = false;
        }

        public class Info
        {
            ///<summary>
            /// Context
            ///</summary>
            public string ElementType;
            ///<summary>
            /// Login Form
            ///</summary>
            public string Name;
            ///<summary>
            /// Form to do login
            ///</summary>
            public string Description;
            ///<summary>
            /// False
            ///</summary>
            public bool IsKey;
            ///<summary>
            /// null
            ///</summary>
            public string InnerKey;
            ///<summary>
            /// Element Locator values
            ///</summary>
            public Locator Locator;
        }

        public class Locator
        {
            ///<summary>
            /// WebLocatorType.XPath
            ///</summary>
            public WebLocatorType LocatorType;
            ///<summary>
            /// .
            ///</summary>
            public string LocatorValue;
            ///<summary>
            /// False
            ///</summary>
            public bool IsRelative;
        }
    }

    ///<summary>
    /// Input for Email
    ///</summary>
    public _Email_Input Email_Input
    {
        get;
        set;
    }

    ///<summary>
    /// Form to do login
    ///</summary>
    public _Login_Form()
    {
        Info = new _Info.Info();
        Locator = new WebLocatorInfo();
        Locator.LocatorType = _Info._Locator.LocatorType;
        Locator.LocatorValue = _Info._Locator.LocatorValue;
        Locator.IsRelative = _Info._Locator.IsRelative;
        Elements = new List<WebElementInfo>();
        Email_Input = new _Email_Input();
        Email_Input.Parent = this;
        Elements.Add(Email_Input);
    }

    ///<summary>
    /// Input for Email
    ///</summary>
    public class _Email_Input : WebElementInfo
    {
        ///<summary>
        /// Information about element
        ///</summary>
        public _Info.Info Info
        {
            get;
            set;
        }

        ///<summary>
        /// Get new element instance
        ///</summary>
        public static _Email_Input GetInstance(CombinedWebElementInfo parentElement) => new _Email_Input{Parent = parentElement};
        public static class _Info
        {
            public const string ElementType = "Element";
            public const string Name = "Email Input";
            public const string Description = "Input for Email";
            public const bool IsKey = false;
            public const string InnerKey = null;
            public static class _Locator
            {
                public const WebLocatorType LocatorType = WebLocatorType.XPath;
                public const string LocatorValue = "//input[@id='email']";
                public const bool IsRelative = false;
            }

            public class Info
            {
                ///<summary>
                /// Element
                ///</summary>
                public string ElementType;
                ///<summary>
                /// Email Input
                ///</summary>
                public string Name;
                ///<summary>
                /// Input for Email
                ///</summary>
                public string Description;
                ///<summary>
                /// False
                ///</summary>
                public bool IsKey;
                ///<summary>
                /// null
                ///</summary>
                public string InnerKey;
                ///<summary>
                /// Element Locator values
                ///</summary>
                public Locator Locator;
            }

            public class Locator
            {
                ///<summary>
                /// WebLocatorType.XPath
                ///</summary>
                public WebLocatorType LocatorType;
                ///<summary>
                /// //input[@id='email']
                ///</summary>
                public string LocatorValue;
                ///<summary>
                /// False
                ///</summary>
                public bool IsRelative;
            }
        }

        ///<summary>
        /// Input for Email
        ///</summary>
        public _Email_Input()
        {
            Info = new _Info.Info();
            Locator = new WebLocatorInfo();
            Locator.LocatorType = _Info._Locator.LocatorType;
            Locator.LocatorValue = _Info._Locator.LocatorValue;
            Locator.IsRelative = _Info._Locator.IsRelative;
        }
    }
}