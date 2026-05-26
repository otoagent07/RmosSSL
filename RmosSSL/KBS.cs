using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace RmosSSL
{
    internal class KBS
    {
        private KBSWebClient client;

        public Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            base64String = base64String.Replace("data:image/png;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }


        string captcha_path = @"C:\u\sr\e-devlet\RetCap.png";
        public void captchaIndir(string htmlAllText)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlAllText);
            string val = doc.DocumentNode.SelectSingleNode("//*[@id=\"imgKod\"]").Attributes["src"].Value; // //*[@id="imgKod"]
            Base64ToImage(val).Save(captcha_path);
            Process.Start(captcha_path);
            //kBSWebClient.DownloadFile("https://kbs.egm.gov.tr/RetCap.aspx", captcha_path);
        }

        string txt;
        int CfgObj, OcrObj, ImgObj;

        public string captchaIndirVeCoz(string htmlAllText)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlAllText);
            string val = doc.DocumentNode.SelectSingleNode("//*[@id=\"imgKod\"]").Attributes["src"].Value; // //*[@id="imgKod"]
            Base64ToImage(val).Save(captcha_path);
            string tamPath = captcha_path;
            NSOCRLib.NSOCRClass NsOCR = new NSOCRLib.NSOCRClass();
            NsOCR.Engine_SetLicenseKey("AB2A4DD5FF2A");
            NsOCR.Engine_InitializeAdvanced(out CfgObj, out OcrObj, out ImgObj);
            NsOCR.Img_LoadFile(ImgObj, tamPath);
            NsOCR.Cfg_SetOption(CfgObj, TNSOCR.BT_DEFAULT, "Languages/Turkish", "1");
            NsOCR.Cfg_SetOption(CfgObj, TNSOCR.BT_DEFAULT, "ImgAlizer/Inversion", "4");
            NsOCR.Img_OCR(ImgObj, TNSOCR.OCRSTEP_FIRST, TNSOCR.OCRSTEP_LAST, TNSOCR.OCRFLAG_NONE);
            NsOCR.Img_GetImgText(ImgObj, out txt, TNSOCR.FMT_EDITCOPY);
            NsOCR.Engine_Uninitialize();
            Console.WriteLine(txt);
            return txt;
        }


        Api2 api = new Api2();

        public void connect(string username, string password, string kod, string otoKod = null)
        {
            using (KBSWebClient kBSWebClient = new KBSWebClient())
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                try
                {
                    api = new Api2();
                    SessionModel session = api.getSession(kBSWebClient);

                }
                catch (Exception ex)
                {

                }

                Console.WriteLine("kullanici : " + username + " şifre = " + password + " kod  = " + kod);


                int sayac = 0;
                string girdi = "Yeni oturum başlatmak için";
                while (girdi.Contains("Yeni oturum başlatmak için"))
                {
                    NameValueCollection data = new NameValueCollection();
                    data.Add("username", username);
                    data.Add("tc", password);
                    data.Add("password", kod);
                    data.Add("vhost", "standard");


                    Console.WriteLine("LÜTFEN BEKLEYİNİZ....");

                    string deger2 = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/my.policy", data));


                    Console.WriteLine("SİSTEME GİRİLİYOR....");


                    if (deger2.Contains("Tek Kullanımlık Parola"))
                    {
                        string sonuc;
                        if (!string.IsNullOrEmpty(otoKod))
                        {
                            sonuc = otoKod;
                        }
                        else
                        {
                            Console.WriteLine("Lütfen Güvenlik Kodunu Girdikten Sonra Entere Basınız.");
                            sonuc = Console.ReadLine();
                        }

                        NameValueCollection data2 = new NameValueCollection();
                        data2.Add("token", sonuc);
                        data2.Add("vhost", "standard");


                        girdi = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/my.policy", data2));

                        Console.WriteLine(sayac + " KERE DENENDİ ");

                        if (girdi.Contains("Hatalı tek kullanımlık parola"))
                        {
                            sayac++;
                            continue;
                        }
                        else
                        {
                            // girdi = kBSWebClient.DownloadString("https://kbs.egm.gov.tr/dana/home/launch.cgi?url=.ahuvsw%3A%2F%2Fsk2Kqt0Ow5BSBA");
                        }
                        if (sayac == 7)
                        {
                            Console.WriteLine("ÇOK DENEME OLDU LÜTFEN KAPATIP AÇIN");
                            Thread.Sleep(10000);
                            break;
                        }
                    }
                    else if (deger2.Contains("Oturum açılamadı"))
                    {
                        api = new Api2();
                        SessionModel session = api.getSession(kBSWebClient);
                    }
                    else if (deger2.Contains("Yeni oturum başlatmak için"))
                    {
                        api = new Api2();
                        SessionModel session = api.getSession(kBSWebClient);
                    }

                }


                this.client = kBSWebClient;
            }
        }

        public List<Guest> guestsbironceki() // 07.07.2023
        {
            List<Guest> list = new List<Guest>();
            string input = this.client.UploadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", "value=asdasd");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


            NameValueCollection data = new NameValueCollection
            {
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                {
                    "btnExcel",
                    "Excel Aktar"
                }
            };
            string @string = Encoding.GetEncoding("iso-8859-9").GetString(this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data));
            MatchCollection matchCollection = Regex.Matches(@string, "<tr[^>]*>(.*?)</tr>", RegexOptions.Singleline);
            int num = 0;
            foreach (Match match in matchCollection)
            {
                if (num == 0)
                {
                    num++;
                }
                else
                {
                    #region 12.12.2017 - Değiştirme Öncesi
                    //    MatchCollection matchCollection2 = Regex.Matches(match.Value, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline);
                    //    bool flag = matchCollection2[5].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "").Length > 0;
                    //    Guest guest = new Guest();
                    //    guest.KEY = matchCollection2[4].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    guest.ROOM = KBS.GetNumbers(matchCollection2[8].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", ""));
                    //    guest.NAME = matchCollection2[0].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    guest.SURNAME = matchCollection2[6].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    guest.COUNTRY = matchCollection2[7].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    guest.CHECKIN = matchCollection2[3].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    guest.PLATE = matchCollection2[1].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //    if (flag)
                    //    {
                    //        guest.ID = matchCollection2[5].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //        guest.setTC();
                    //    }
                    //    else
                    //    {
                    //        guest.ID = matchCollection2[2].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //        guest.setForeign();
                    //    }
                    //    list.Add(guest);
                    //    num++; 
                    #endregion

                    // 07.07.2023 oncesi
                    MatchCollection matchCollection2 = Regex.Matches(match.Value, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline);
                    bool flag = matchCollection2[6].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "").Length > 0;
                    Guest guest = new Guest();
                    guest.KEY = matchCollection2[5].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.ROOM = KBS.GetNumbers(matchCollection2[9].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", ""));
                    guest.NAME = matchCollection2[0].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.SURNAME = matchCollection2[7].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.COUNTRY = matchCollection2[8].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.CHECKIN = matchCollection2[4].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.PLATE = matchCollection2[1].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //guest.GENDER = matchCollection2[2].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    if (flag)
                    {
                        guest.ID = matchCollection2[6].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                        guest.setTC();
                    }
                    else
                    {
                        guest.ID = matchCollection2[3].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                        guest.setForeign();
                    }
                    list.Add(guest);
                    num++;

                }
            }
            return list;
        } // 07.07.2023




        public string coz(string input)
        {
            try
            {
                byte[] latin1Bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
                return Encoding.UTF8.GetString(latin1Bytes);
            }
            catch (Exception ex)
            {
                return input;
            }
         
        }

        public List<Guest> guests() // 12.06.2023 30.04.2025 11
        {
            List<Guest> list = new List<Guest>();
            string input = this.client.UploadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", "value=asdasd");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            //string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;




            //this.client.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-9");
            client.Encoding = Encoding.UTF8; // ← Burası çok önemli!
            var datam = this.client.DownloadData("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx");
            string text = Encoding.UTF8.GetString(datam); // BU DOĞRU

           


        etiket:

            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
            doc2.LoadHtml(text);

            string AntiForgeryToken = doc2.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;
            value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            var node = doc2.DocumentNode.SelectNodes("//*[@id=\"grdkonaklayan\"]");
            if (node == null)
            {
                Console.WriteLine("misafir bilgisi alınamadı!!!");

            }
            else
            {
                bool ilkSatir = true;
                foreach (var row1 in node)
                {

                    foreach (var row2 in row1.SelectNodes("./tr"))
                    {
                        string oda = "";
                        try
                        {
                            if (ilkSatir)
                            {
                                ilkSatir = false;
                                continue;
                            }
                            string varmi = row2.InnerText.Replace("\r\n", "").Trim();
                            if (varmi == "") continue;
                            string key = row2.SelectSingleNode("./td[2]/input").Attributes["value"].Value.Replace("\r\n", "").Trim();
                            string tc = row2.SelectSingleNode("./td[2]").InnerText.Replace("\r\n", "").Trim();
                            string pasaport = row2.SelectSingleNode("./td[3]").InnerText.Replace("\r\n", "").Trim();
                            string ad = row2.SelectSingleNode("./td[4]").InnerText.Replace("\r\n", "").Trim();
                            string soyad = row2.SelectSingleNode("./td[5]").InnerText.Replace("\r\n", "").Trim();
                            string aracplaka = row2.SelectSingleNode("./td[6]").InnerText.Replace("\r\n", "").Trim();
                            string giristar = row2.SelectSingleNode("./td[7]").InnerText.Replace("\r\n", "").Trim();
                             oda = row2.SelectSingleNode("./td[8]").InnerText.Replace("\r\n", "").Trim();

                            oda = Regex.Replace(oda, @"\D", "");


                            string ulke = row2.SelectSingleNode("./td[9]").InnerText.Replace("\r\n", "").Trim();
                            string cinsiyet = row2.SelectSingleNode("./td[10]").InnerText.Replace("\r\n", "").Trim();

                            Guest guest = new Guest();
                            guest.KEY = key;
                            guest.ROOM = Convert.ToInt32(oda);
                            guest.NAME = coz(ad);
                            guest.SURNAME = coz(soyad);
                            guest.COUNTRY = ulke;
                            guest.CHECKIN = giristar;
                            guest.PLATE = aracplaka;
                            //guest.GENDER = matchCollection2[2].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                            if (tc != "")
                            {
                                guest.ID = tc;
                                guest.setTC();
                            }
                            else
                            {
                                guest.ID = pasaport;
                                guest.setForeign();

                            }
                            list.Add(guest);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"bu odada sorun var : {oda}");
                        }
                        

                    }
                }
            }


            value = Regex.Matches(text, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            NameValueCollection data = new NameValueCollection
            {
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                {
                    "__EVENTTARGET",
                    "grdkonaklayan"
                },
                {
                    "__EVENTARGUMENT",
                    "Page$Next"
                }
            };

            string json = JsonConvert.SerializeObject(list);
            if (text.Contains("alt=\"Sonraki Sayfa\""))
            {
                //string jsontext = JsonConvert.SerializeObject(list);
                Console.WriteLine(list.Count + " Veri çekildi...");

                text = Encoding.GetEncoding("iso-8859-9").GetString(this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data));


                goto etiket;
            }
            else
            {
                Console.WriteLine("Polis Sitesindeki Toplam Misafir Sayısı : " + list.Count);


                return list;

            }
        }





        public List<Guest> guestsyeni2() // 10.06.2023
        {
            List<Guest> list = new List<Guest>();
            string input = this.client.UploadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", "value=asdasd");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


            NameValueCollection data = new NameValueCollection
            {
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                {
                    "btnExcel",
                    "Excel Aktar"
                }
            };

            //string @string = Encoding.GetEncoding("iso-8859-9").GetString(this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data));
            this.client.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-9");
            var datam = this.client.DownloadData("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx");
            string text = Encoding.UTF8.GetString(datam);
            MatchCollection matchCollection = Regex.Matches(text, "<tr[^>]*>(.*?)</tr>", RegexOptions.Singleline);
            int num = 0;
            foreach (Match match in matchCollection)
            {
                if (num == 0)
                {
                    num++;
                }
                else
                {
                   

                    MatchCollection matchCollection2 = Regex.Matches(match.Value, "<td[^>]*>(.*?)</td>", RegexOptions.Singleline);
                    bool flag = matchCollection2[6].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "").Length > 0;
                    Guest guest = new Guest();
                    guest.KEY = matchCollection2[5].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.ROOM = KBS.GetNumbers(matchCollection2[9].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", ""));
                    guest.NAME = matchCollection2[0].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.SURNAME = matchCollection2[7].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.COUNTRY = matchCollection2[8].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.CHECKIN = matchCollection2[4].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    guest.PLATE = matchCollection2[1].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    //guest.GENDER = matchCollection2[2].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                    if (flag)
                    {
                        guest.ID = matchCollection2[6].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                        guest.setTC();
                    }
                    else
                    {
                        guest.ID = matchCollection2[3].Groups[1].Value.Replace("nbsp;", "").Replace("&amp;", "").Replace("&", "");
                        guest.setForeign();
                    }
                    list.Add(guest);
                    num++;

                }
            }
            return list;
        }

        public void addTCGuest(string GuestTC, int GuestRoom, string GuestPlate = null, bool GuestDETAIL = false)
        {
            string input = this.client.UploadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", "value=value");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


            NameValueCollection data = new NameValueCollection
            {
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "__SCROLLPOSITIONX",
                    "0"
                },
                {
                    "__SCROLLPOSITIONY",
                    "0"
                },
                {
                    "imgmernis.x",
                    "21"
                },
                {
                    "imgmernis.y",
                    "15"
                },
                {
                    "txttcno",
                    GuestTC
                }
            };
            byte[] bytes = this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
            if (Regex.Match(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
            {
                string sad = Encoding.UTF8.GetString(bytes);
                value = Regex.Matches(Encoding.UTF8.GetString(bytes), "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                value2 = Regex.Matches(Encoding.UTF8.GetString(bytes), "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value3 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value4 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtSoyadi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value5 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtBabaAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value6 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAnaAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value7 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtDogum\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value8 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtDogumYeri\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value9 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtGelisTarihi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value10 = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtCinsiyet\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

                value8 = "Turkiye";

                if (!GuestDETAIL)
                {
                    data = new NameValueCollection
                    {
                        {
                    "AntiForgeryToken",
                    AntiForgeryToken
                        },
                        {
                            "__VIEWSTATE",
                            value
                        },
                        {
                            "__VIEWSTATEGENERATOR",
                            value2
                        },
                        {
                            "__SCROLLPOSITIONX",
                            "0"
                        },
                        {
                            "__SCROLLPOSITIONY",
                            "0"
                        },
                        {
                            "txttcno",
                            GuestTC
                        },
                        {
                            "txtAdi",
                            value3
                        },
                        {
                            "txtSoyadi",
                            value4
                        },
                        {
                            "txtBabaAdi",
                            value5
                        },
                        {
                            "txtAnaAdi",
                            value6
                        },
                        {
                            "txtDogum",
                            value7
                        },
                        {
                            "txtDogumYeri",
                            value8
                        },
                        {
                            "txtGelisTarihi",
                            value9
                        },
                        {
                            "txtCinsiyet",
                            value10
                        },
                        {
                            "txtAracPlaka",
                            GuestPlate
                        },
                        {
                            "txtVerilenOda",
                            GuestRoom.ToString()
                        },
                        {
                            "btnTurk",
                            "Kaydet"
                        }
                    };
                    this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
                }
                else
                {
                    data = new NameValueCollection
                    {
                        {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                        {
                            "__VIEWSTATE",
                            value
                        },
                        {
                            "__VIEWSTATEGENERATOR",
                            value2
                        },
                        {
                            "__SCROLLPOSITIONX",
                            "0"
                        },
                        {
                            "__SCROLLPOSITIONY",
                            "0"
                        },
                        {
                            "txttcno",
                            GuestTC
                        },
                        {
                            "txtAdi",
                            value3
                        },
                        {
                            "txtSoyadi",
                            value4
                        },
                        {
                            "txtBabaAdi",
                            value5
                        },
                        {
                            "txtAnaAdi",
                            value6
                        },
                        {
                            "txtDogum",
                            value7
                        },
                        {
                            "txtDogumYeri",
                            value8
                        },
                        {
                            "txtGelisTarihi",
                            value9
                        },
                        {
                            "txtCinsiyet",
                            value10
                        },
                        {
                            "txtAracPlaka",
                            GuestPlate
                        },
                        {
                            "txtVerilenOda",
                            GuestRoom.ToString()
                        },
                        {
                            "btnTurkAyniOda",
                            "Kaydet"
                        }
                    };
                    this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
                }




                // 30.04.2025 de aynı odada kalsınmı uyarısına evet dedik
            


            }
        }

        public void addForeignGuest(string GuestID, string GuestName, string GuestSurname, string GuestBirth, string GuestCountry, string GuestNation, int GuestRoom, string GuestPlate = null, string GuestFather = null, string GuestMother = null, string GuestGender = null, bool GuestDETAIL = false)
        {
            string input = this.client.DownloadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value3 = Regex.Matches(input, "<input[^>]*name=\"txtYGelisTarihi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


            if (!GuestDETAIL)
            {
                NameValueCollection data = new NameValueCollection
                {
                    {
                        "__VIEWSTATE",
                        value
                    },
                    {
                        "__VIEWSTATEGENERATOR",
                        value2
                    },
                    {
                        "__SCROLLPOSITIONX",
                        "0"
                    },
                    {
                        "__SCROLLPOSITIONY",
                        "1020"
                    },
                    {
                        "txtPasaport",
                        GuestID
                    },
                    {
                        "txtYAdi",
                        GuestName
                    },
                    {
                        "txtYSoyadi",
                        GuestSurname
                    },
                    {
                        "txtYBabaadi",
                        GuestFather
                    },
                    {
                        "txtYAnaadi",
                        GuestMother
                    },
                    {
                        "txtYDogum",
                        GuestBirth
                    },
                    {
                        "txtYDogumYeri",
                        GuestCountry
                    },
                    {
                        "drpUyrugu",
                        GuestNation
                    },
                    {
                        "drp_listCinsiyet",
                        GuestGender
                    },
                    {
                        "txtYGelisTarihi",
                        value3
                    },
                    {
                        "txtYAracPlaka",
                        GuestPlate
                    },
                    {
                        "txtYVerilenOda",
                        GuestRoom.ToString()
                    },
                    {
                        "btnYabanci",
                        "Kaydet"
                    },
                    {
                        "AntiForgeryToken",
                        AntiForgeryToken
                    }
                };
                this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
            }
            else
            {
                NameValueCollection data = new NameValueCollection
                {
                    {
                    "AntiForgeryToken",
                    AntiForgeryToken
                },
                    {
                        "__VIEWSTATE",
                        value
                    },
                    {
                        "__VIEWSTATEGENERATOR",
                        value2
                    },
                    {
                        "__SCROLLPOSITIONX",
                        "0"
                    },
                    {
                        "__SCROLLPOSITIONY",
                        "1020"
                    },
                    {
                        "txtPasaport",
                        GuestID
                    },
                    {
                        "txtYAdi",
                        GuestName
                    },
                    {
                        "txtYSoyadi",
                        GuestSurname
                    },
                    {
                        "txtYBabaadi",
                        GuestFather
                    },
                    {
                        "txtYAnaadi",
                        GuestMother
                    },
                    {
                        "txtYDogum",
                        GuestBirth
                    },
                    {
                        "txtYDogumYeri",
                        GuestCountry
                    },
                    {
                        "drpUyrugu",
                        GuestNation
                    },
                    {
                        "drp_listCinsiyet",
                        GuestGender
                    },
                    {
                        "txtYGelisTarihi",
                        value3
                    },
                    {
                        "txtYAracPlaka",
                        GuestPlate
                    },
                    {
                        "txtYVerilenOda",
                        GuestRoom.ToString()
                    },
                    {
                        "btnYabanciAyniOda",
                        "Kaydet"
                    }
                };
                this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
            }


        }

      

        public void checkout(string KEY)
        {
            try
            {



                //string input = this.client.DownloadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx");
                string input = this.client.UploadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", "value=value");

                string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;


                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(input);
                string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


                NameValueCollection data = new NameValueCollection
            {
                {
                    "__EVENTTARGET",
                    "grdkonaklayan"
                },
                {
                    "__EVENTARGUMENT",
                    "Delete$0"
                },
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "__SCROLLPOSITIONX",
                    "439"
                },
                {
                    "__SCROLLPOSITIONY",
                    "1190"
                },
                {
                    "SelectedIndex",
                    KEY
                },
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                }
            };


                this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);

            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA -> KBS.checkout() " + ex.Message);
            }

        }

        public static int GetNumbers(string input)
        {
            return int.Parse(new string((from c in input
                                         where char.IsDigit(c)
                                         select c).ToArray<char>()));
        }

        public KBSPerson TC_Sorgula(string GuestTC)
        {
            string input = this.client.DownloadString("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx");
            string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
            string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(input);
            string AntiForgeryToken = doc.DocumentNode.SelectSingleNode("//*[@id=\"AntiForgeryToken\"]").Attributes["value"].Value;


            NameValueCollection data = new NameValueCollection
            {
                {
                    "__VIEWSTATE",
                    value
                },
                {
                    "__VIEWSTATEGENERATOR",
                    value2
                },
                {
                    "__SCROLLPOSITIONX",
                    "0"
                },
                {
                    "__SCROLLPOSITIONY",
                    "0"
                },
                {
                    "imgmernis.x",
                    "21"
                },
                {
                    "imgmernis.y",
                    "15"
                },
                {
                    "txttcno",
                    GuestTC
                },
                {
                    "AntiForgeryToken",
                    AntiForgeryToken
                }
            };
            byte[] bytes = this.client.UploadValues("https://kbs.egm.gov.tr/ProjeDosya/konaklayanekle.aspx", data);
            if (Regex.Match(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
            {
                value = Regex.Matches(Encoding.UTF8.GetString(bytes), "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                value2 = Regex.Matches(Encoding.UTF8.GetString(bytes), "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string Adi = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string Soyadi = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtSoyadi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string BabaAdi = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtBabaAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string AnaAdi = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtAnaAdi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string Dogum = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtDogum\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string DogumYeri = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtDogumYeri\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                string GelisTarihi = Regex.Matches(Encoding.UTF8.GetString(bytes), "<input[^>]*name=\"txtGelisTarihi\"[^>]*value=\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;


                KBSPerson g = new KBSPerson
                {
                    TC_No = GuestTC,
                    Adi = Adi,
                    Soyadi = Soyadi,
                    Baba_Adi = BabaAdi,
                    Ana_Adi = AnaAdi,
                    Dogum_Tarihi = Dogum,
                    Dogum_Yeri = DogumYeri
                };

                return g;
            }
            else
            {
                return null;
            }

        }
    }
}
