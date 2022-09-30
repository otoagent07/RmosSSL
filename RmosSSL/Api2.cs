using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RmosSSL
{
    public class Api2
    {
        HttpClient client;
        CookieContainer cookies ;

        public Api2()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClientHandler handler = new HttpClientHandler();
            cookies = new CookieContainer();
            handler.CookieContainer = cookies;
            client = new HttpClient(handler);
        }

        public void apiSifirla()
        {
            cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            client = new HttpClient(handler);
        }
        public string requestPost(string url, Dictionary<string, string> dict)
        {
            apiSifirla();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            var Content = new FormUrlEncodedContent(dict);
            HttpResponseMessage responseOtel = client.PostAsync(url, Content).Result;
            string result = responseOtel.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string requestPostJson(string url, object model)
        {
            apiSifirla();

            string json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseOtel = client.PostAsync(url, stringContent).Result;
            string result = responseOtel.Content.ReadAsStringAsync().Result;
            return result;
        }

        public string requestGet(string url)
        {
            apiSifirla();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            HttpResponseMessage responseOtel = client.GetAsync(url).Result;
            string result = responseOtel.Content.ReadAsStringAsync().Result;
            result = System.Net.WebUtility.HtmlDecode(result);

            return result;
        }

        public string requestGetBase64(string apiKey, int carId)
        {
            string url = "resimurl";

            HttpResponseMessage response = client.GetAsync(url).Result;
            var result = response.Content.ReadAsByteArrayAsync().Result;
            return Convert.ToBase64String(result);
        }


        public string kes(string txt, string ilk, string son)
        {
            try
            {
                string parcali1 = (txt.Split(new string[] { ilk }, StringSplitOptions.None))[1];
                string parcali2 = (parcali1.Split(new string[] { son }, StringSplitOptions.None))[0];
                return parcali2.Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> kesList(string txt, string ilk, string son)
        {
            try
            {
                List<string> list = new List<string>();
                string[] parcali1 = (txt.Split(new string[] { ilk }, StringSplitOptions.None));
                for (int i = 1; i < parcali1.Length; i++)
                {
                    string parcali2 = (parcali1[i].Split(new string[] { son }, StringSplitOptions.None))[0];
                    list.Add(parcali2.Trim());

                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public SessionModel getSession(KBSWebClient webClient)
        {
            apiSifirla();

            requestGet("https://kbs.egm.gov.tr/");
            SessionModel sessionModel = new SessionModel();
            foreach (Cookie item in cookies.GetCookies(new Uri("https://kbs.egm.gov.tr")))
            {
                webClient._cookieContainer.Add(item);

                switch (item.Name)
                {
                    case "LastMRH_Session":
                        sessionModel.LastMRH_Session = item.Value;
                        break;
                    case "MRHSession":
                        sessionModel.MRHSession = item.Value;
                        break;
                }
                
            }

            return sessionModel;
        }


    }
}
