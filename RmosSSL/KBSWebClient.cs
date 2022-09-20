using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace RmosSSL
{
    public class KBSWebClient : WebClient
    {
        private CookieContainer _cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            WebRequest webRequest = base.GetWebRequest(address);
            if (webRequest is HttpWebRequest)
            {
                (webRequest as HttpWebRequest).CookieContainer = this._cookieContainer;
            }
            string str = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\";
            if (File.Exists(str + "proxy.xml"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(str + "proxy.xml");
                string ınnerText = xmlDocument.DocumentElement.SelectSingleNode("server").InnerText;
                string ınnerText2 = xmlDocument.DocumentElement.SelectSingleNode("username").InnerText;
                string ınnerText3 = xmlDocument.DocumentElement.SelectSingleNode("password").InnerText;
                webRequest.Proxy = new WebProxy(ınnerText)
                {
                    Credentials = new NetworkCredential(ınnerText2, ınnerText3),
                    UseDefaultCredentials = false,
                    BypassProxyOnLocal = false
                };
            }
            return webRequest;
        }
    }
}
