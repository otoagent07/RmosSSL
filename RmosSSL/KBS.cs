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


        string captcha_path = @"C:\u\sr\e-devlet\RetCap.aspx";
        public void captchaIndir(string htmlAllText)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlAllText);
            string val = doc.DocumentNode.SelectSingleNode("//*[@id=\"imgKod\"]").Attributes["src"].Value; // //*[@id="imgKod"]
            Base64ToImage(val).Save(captcha_path);

            //kBSWebClient.DownloadFile("https://kbs.egm.gov.tr/RetCap.aspx", captcha_path);
        }


        Api2 api = new Api2();

        public void connect(string username, string password, string kod)
        {
            using (KBSWebClient kBSWebClient = new KBSWebClient())
            {
                // string input = kBSWebClient.DownloadString("http://kbs.egm.gov.tr/login.aspx");
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
                    string input = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/login.aspx", new NameValueCollection()));

                    string value = Regex.Matches(input, "id=\\\"__VIEWSTATE\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;
                    string value2 = Regex.Matches(input, "id=\\\"__VIEWSTATEGENERATOR\\\" value=\\\"(.*?)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline)[0].Groups[1].Value;



                    NameValueCollection data = new NameValueCollection(); // password
                    data.Add("txtkullaniciadi", username);
                    data.Add("txtsifre", kod);
                    data.Add("Button1", "Giriş");
                    data.Add("GuidId", "");
                    data.Add("__EVENTTARGET", "");
                    data.Add("__EVENTARGUMENT", "");
                    data.Add("__VIEWSTATE", value);
                    data.Add("__VIEWSTATEGENERATOR", value2);
                    data.Add("AntiForgeryToken", "");


                    Console.WriteLine("LÜTFEN BEKLEYİNİZ....");

                    string deger2 = "";//Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/my.policy", data));


                    Console.WriteLine("SİSTEME GİRİLİYOR....");

                    if (1==1)
                    {
                        Console.WriteLine("Lütfen Güvenlik Kodunu Girdikten Sonra Entere Basınız.");
                        string sonuc = Console.ReadLine();

                        //NameValueCollection data2 = new NameValueCollection();
                        data.Add("txtCaptcha", sonuc);
                        //data2.Add("vhost", "standard");


                        girdi = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/login.aspx", data));

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


        /*
        public void connect(string username, string password, string kod)
        {
            using (KBSWebClient kBSWebClient = new KBSWebClient())
            {
                // string input = kBSWebClient.DownloadString("http://kbs.egm.gov.tr/login.aspx");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                try
                {
                    string cikisYapmak = kBSWebClient.DownloadString("https://kbs.egm.gov.tr/dana-na/auth/logout.cgi?delivery=psal");
                }
                catch (Exception ex)
                {

                }

                Console.WriteLine("kullanici : " + username + " şifre = " + password + " kod  = " + kod);


                int sayac = 0;
                string girdi = "Invalid secondary username or password. Please re-enter your user information";
                while (girdi.Contains("Invalid secondary username or password. Please re-enter your user information"))
                {
                    NameValueCollection data = new NameValueCollection
                {
                    {
                        "tz_offset",
                        ""
                    },
                    {
                        "clientMAC",
                        ""
                    },
                    {
                        "password2",
                        username
                    },
                    {
                        "tckimlik",
                        password
                    },

                    {
                        "password",
                        kod
                    },
                    {
                        "username",
                        ""+username+"+,+"+password
                    },
                    {
                        "realm",
                        "KBS_REALM"
                    },
                    {
                        "btnSubmit",
                        "Giriş"
                    }
                };

                    Console.WriteLine("LÜTFEN BEKLEYİNİZ....");

                    string deger2 = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/dana-na/auth/url_CACp8c5rMVjMp7z4/login.cgi", data));


                    Console.WriteLine("SİSTEME GİRİLİYOR....");

                    if (!deger2.Contains("İkincil parola:"))
                    {
                        //Console.WriteLine("Kullanıcı Adı veya  Şifre  Hatalı Girildi! \n Lütfen ayarlardan düzeltip relogin yapınız !");
                        deger2 = "https://www.egm.gov.tr/";

                    }
                    else
                    {
                        int state1 = deger2.IndexOf("state_");
                        string kes = deger2.Substring(state1, deger2.Length - state1);
                        kes = kes.Substring(0, kes.IndexOf("\""));
                        Console.WriteLine("Lütfen Güvenlik Kodunu Girdikten Sonra Entere Basınız.");
                        string sonuc = Console.ReadLine();

                        NameValueCollection data2 = new NameValueCollection
                {
                    {
                        "key",
                        kes
                    },
                    {
                        "password#2",
                        sonuc
                    },
                    {
                        "btnSubmit",
                        "Giriş"
                    },

                    };


                        girdi = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/dana-na/auth/url_CACp8c5rMVjMp7z4/login.cgi", data2));




                        Console.WriteLine(sayac + " KERE DENENDİ ");


                        if (sayac == 7)
                        {
                            Console.WriteLine("ÇOK DENEME OLDU LÜTFEN KAPATIP AÇIN");
                            Thread.Sleep(10000);
                            break;
                        }


                        if (girdi.Contains("Invalid secondary username or password. Please re-enter your user information")) continue;


                        if (girdi.Contains("There are already other user sessions in progress:"))
                        {
                            File.WriteAllText("tikla.html", girdi);

                            int index1 = girdi.IndexOf("value=\"193;320;");

                            string formdatastr = girdi.Substring(index1, girdi.Length - index1).Replace("value=\"", "");
                            index1 = formdatastr.IndexOf("\">");
                            formdatastr = formdatastr.Substring(0, index1);

                            NameValueCollection data3 = new NameValueCollection
                {
                    {
                        "btnContinue",
                        "Continue the session"
                    },
                    {
                        "FormDataStr",
                        formdatastr
                    }
                };

                            string deger3 = Encoding.UTF8.GetString(kBSWebClient.UploadValues("https://kbs.egm.gov.tr/dana-na/auth/url_CACp8c5rMVjMp7z4/login.cgi", data3));

                            Console.WriteLine("SİSTEME GİRİLDİ BEKLEYİNİZ...");
                        }


                        girdi = kBSWebClient.DownloadString("https://kbs.egm.gov.tr/dana/home/launch.cgi?url=.ahuvsw%3A%2F%2Fsk2Kqt0Ow5BSBA");


                    }
                }


                this.client = kBSWebClient;
            }
        }

        */
        public List<Guest> guests()
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

                //DateTime tarih = DateTime.Now;
                //DateTime.TryParseExact(Dogum, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);


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
