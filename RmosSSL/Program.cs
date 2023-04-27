using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace RmosSSL
{
    public class Program
    {
        //static string RMOS_CONNSTR = "";
        static void Main(string[] args)
        {
            //Yukle("", "", @"C:\444\P_19790_21_12_2017.xml");
            //Console.ReadLine();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            //args = new string[5];
            //args[0] = "muhasebe@sealifehotels.com";// 
            //args[1] = "13430589946";// "434528"; 
            //args[2] = "keykubat";
            //args[3] = @"C:\cc\P_193602_30_09_2022.xml";
            //args[4] = @"207469";


            if (args.Length < 4)
            {
                Console.WriteLine("Hatalı Parametre");
                Console.ReadLine();
                return;
            }

            string KBS_username = args[0];
            string KBS_password = args[1];
            string RMOS_SIFRE = args[2];
            string KBS_Path = args[3];
            string kod = args[4];




            //RMOS_CONNSTR = args[4];

            if (RMOS_SIFRE != "keykubat")
            {
                Console.WriteLine("Geçersiz Lisans !!!");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Dosya : " + KBS_Path);
            Console.WriteLine("Kullanıcı : " + KBS_username);



            Yukle(KBS_username, KBS_password, KBS_Path, kod);

            Thread.Sleep(10000);

            //Console.ReadLine();
        }

        public static Dictionary<int, string> Main(List<Models.PersonelModel> pers)
        {
            Dictionary<int, string> sonuc = new Dictionary<int, string>();

            foreach (var per in pers)
            {
                sonuc.Add(Array.IndexOf(pers.ToArray(), per), per.ad);
            }

            return sonuc;
        }

        private static void Yukle(string KBS_username, string KBS_password, string KBS_Path, string kod)
        {
            try
            {
                #region nations
                string[] nations = new string[]
                {
            "NONE",         //0
            "TC",           //1
            "KKTC",         //2
            "AFG",          //3
            "ALB",          //4
            "DZA",          //5
            "ASM",          //6
            "AND",          //7
            "AGO",          //8
            "AIA",          //9
            "ATA",          //10
            "ATG",          //11
            "ARG",          //12
            "ARM",          //13
            "ABW",          //14
            "AUS",          //15
            "AUT",          //16
            "AZE",          //17
            "BHS",          //18
            "BHR",          //19
            "BGD",          //20
            "BRB",          //21
            "BLR",          //22
            "BEL",          //23
            "BLZ",          //24
            "BEN",          //25
            "BMU",          //26
            "BTN",          //27
            "BOL",          //28
            "BIH",          //29
            "BWA",          //30
            "BVT",          //31
            "BRA",          //32
            "IOT",          //33
            "BRN",          //34
            "BGR",          //35
            "BFA",          //36
            "BDI",          //37
            "KHM",          //38
            "CMR",          //39
            "CAN",          //40
            "CPV",          //41
            "CYM",          //42
            "CAF",          //43
            "TCD",          //44
            "CHL",          //45
            "CHN",          //46
            "CXR",          //47
            "CCK",          //48
            "COL",          //49
            "COM",          //50
            "COG",          //51
            "COG",          //52
            "COK",          //53
            "CRI",          //54
            "CIV",          //55
            "HRV",          //56
            "CUB",          //57
            "CYP",          //58
            "CZE",          //59
            "DNK",          //60
            "DJI",          //61
            "DMA",          //62
            "DOM",          //63
            "ECU",          //64
            "EGY",          //65
            "SLV",          //66
            "GNQ",          //67
            "ERI",          //68
            "EST",          //69
            "ETH",          //70
            "FRO",          //71
            "FLK",          //72
            "FJI",          //73
            "FIN",          //74
            "FRA",          //75
            "GUF",          //76
            "PYF",          //77
            "ATF",          //78
            "GAB",          //79
            "GMB",          //80
            "GEO",          //81
            "DEU",          //82
            "GHA",          //83
            "GIB",          //84
            "GRC",          //85
            "GRL",          //86
            "GRD",          //87
            "GLP",          //88
            "GUM",          //89
            "GTM",          //90
            "GBG",          //91
            "GIN",          //92
            "GNB",          //93
            "GUY",          //94
            "HTI",          //95
            "HMD",          //96
            "HND",          //97
            "HKG",          //98
            "HUN",          //99
            "ISL",          //100
            "IND",          //101
            "IDN",          //102
            "IRN",          //103
            "IRQ",          //104
            "IRL",          //105
            "ISR",          //106
            "ITA",          //107
            "JAM",          //108
            "JPN",          //109
            "GBJ",          //110
            "JOR",          //111
            "KAZ",          //112
            "KEN",          //113
            "KIR",          //114
            "PRK",          //115
            "KOR",          //116
            "KWT",          //117
            "KGZ",          //118
            "LAO",          //119
            "LVA",          //120
            "LBN",          //121
            "LSO",          //122
            "LBR",          //123
            "LBY",          //124
            "LIE",          //125
            "LTU",          //126
            "LUX",          //127
            "MAC",          //128
            "MKD",          //129
            "MDG",          //130
            "MWI",          //131
            "MYS",          //132
            "MDV",          //133
            "MLI",          //134
            "MLT",          //135
            "MHL",          //136
            "MTQ",          //137
            "MRT",          //138
            "MUS",          //139
            "MYT",          //140
            "MEX",          //141
            "FSM",          //142
            "MDA",          //143
            "MCO",          //144
            "MNG",          //145
            "MSR",          //146
            "MAR",          //147
            "MOZ",          //148
            "MMR",          //149
            "NAM",          //150
            "NRU",          //151
            "NPL",          //152
            "NLD",          //153
            "ANT",          //154
            "NCL",          //155
            "NZL",          //156
            "NIC",          //157
            "NER",          //158
            "NGA",          //159
            "NIU",          //160
            "NFK",          //161
            "MNP",          //162
            "NOR",          //163
            "OMN",          //164
            "PAK",          //165
            "PLW",          //166
            "PSE",          //167
            "PAN",          //168
            "PNG",          //169
            "PRY",          //170
            "PER",          //171
            "PHL",          //172
            "PCN",          //173
            "POL",          //174
            "PRT",          //175
            "PRI",          //176
            "QAT",          //177
            "REU",          //178
            "ROU",          //179
            "RUS",          //180
            "RWA",          //181
            "SHN",          //182
            "KNA",          //183
            "LCA",          //184
            "SPM",          //185
            "VCT",          //186
            "WSM",          //187
            "SMR",          //188
            "STP",          //189
            "SAU",          //190
            "SEN",          //191
            "SRB",          //192
            "SYC",          //193
            "SLE",          //194
            "SGP",          //195
            "SVK",          //196
            "SVN",          //197
            "SLB",          //198
            "SOM",          //199
            "ZAF",          //200
            "XXX",          //201   YOK
            "SGS",          //202
            "ESP",          //203
            "LKA",          //204
            "SDN",          //205
            "SUR",          //206
            "SJM",          //207
            "SWZ",          //208
            "SWE",          //209
            "CHE",          //210
            "SYR",          //211
            "TWN",          //212
            "TJK",          //213
            "TZA",          //214
            "THA",          //215
            "TLS",          //216
            "TGO",          //217
            "TKL",          //218
            "TON",          //219
            "TTO",          //220
            "TUN",          //221
            "TKM",          //222
            "TCA",          //223
            "TUV",          //224
            "UGA",          //225
            "UKR",          //226
            "ARE",          //227
            "GBR",          //228
            "USA",          //229
            "UMI",          //230
            "URY",          //231
            "UZB",          //232
            "VUT",          //233
            "VAT",          //234
            "VEN",          //235
            "VEN",          //236
            "VIR",          //237
            "VIR",          //238
            "WLF",          //239
            "ESH",          //240
            "YEM",          //241
            "ZMB",          //242
            "ZIM",          //243
            "KOR"           //244
                };
                #endregion

                string[] genders = new string[]
                {
                    "",
                    "Erkek",
                    "Kadın"
                };

                Thread.Sleep(500);
                KBS KBS = new KBS();
                List<Guest> list = new List<Guest>();
                List<int> list2 = new List<int>();
                if (File.Exists(KBS_Path))
                {
                    Console.WriteLine("Aktarım Başladı...");

                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(KBS_Path);
                        XmlNode xmlNode = xmlDocument.SelectSingleNode("Konaklama");
                        int num = 0;
                        int num2 = 0;
                        if (xmlNode != null)
                        {
                            foreach (XmlNode xmlNode2 in xmlNode)
                            {
                                int numbers = KBS.GetNumbers(xmlNode2.Attributes["SiraNo"].Value);
                                num2++;
                                try
                                {
                                    Guest guest3 = new Guest();
                                    bool flag = xmlNode2.Attributes["TCKimlikNo"].Value.Length == 11;

                                    //if (flag)
                                    //{
                                    //    if (xmlNode2.Attributes["TCKimlikNo"].Value.Substring(0, 2) == "99")
                                    //    {
                                    //        flag = false;
                                    //    }
                                    //}

                                    if (!Regex.IsMatch(xmlNode2.Attributes["VerilenOdaNo"].Value, "^\\d+$"))
                                    {
                                        num++;
                                        list2.Add(numbers);
                                    }
                                    else
                                    {
                                        int numbers2 = KBS.GetNumbers(xmlNode2.Attributes["VerilenOdaNo"].Value);
                                        string text = Regex.Replace(xmlNode2.Attributes["Adi"].Value.ToString(), "\\s+", "");
                                        if (text == "" || text == null)
                                        {
                                            num++;
                                            list2.Add(numbers);
                                        }
                                        else
                                        {
                                            string text2 = Regex.Replace(xmlNode2.Attributes["Soyadi"].Value.ToString(), "\\s+", "");
                                            if (text2 == "" || text2 == null)
                                            {
                                                num++;
                                                list2.Add(numbers);
                                            }
                                            else
                                            {
                                                string pLATE = Regex.Replace(xmlNode2.Attributes["AracPlakaNo"].Value.ToString(), "\\s+", "");
                                                guest3.ROOM = numbers2;
                                                guest3.PLATE = pLATE;
                                                if (flag)
                                                {
                                                    guest3.ID = Regex.Replace(xmlNode2.Attributes["TCKimlikNo"].Value, "\\s+", "");
                                                    guest3.NAME = text;
                                                    guest3.SURNAME = text2;
                                                }
                                                else
                                                {
                                                    string text3 = Regex.Replace(xmlNode2.Attributes["DogumTarihi"].Value.ToString(), "\\s+", "");
                                                    if (text3.Length != 10)
                                                    {
                                                        num++;
                                                        list2.Add(numbers);
                                                        continue;
                                                    }
                                                    string text4 = Regex.Replace(xmlNode2.Attributes["KimlikSeriNo"].Value, "\\s+", "").ToString();
                                                    if (text4 == "" || text4 == null)
                                                    {
                                                        num++;
                                                        list2.Add(numbers);
                                                        continue;
                                                    }
                                                    string text5 = Regex.Replace(xmlNode2.Attributes["Uyrugu"].Value, "\\s+", "").ToString();
                                                    if (text5 == "" || text5 == null)
                                                    {
                                                        num++;
                                                        list2.Add(numbers);
                                                        continue;
                                                    }
                                                    if (Array.IndexOf<string>(nations, xmlNode2.Attributes["Uyrugu"].Value) <= 0 || Array.IndexOf<string>(nations, xmlNode2.Attributes["Uyrugu"].Value) == 1)
                                                    {
                                                        num++;
                                                        list2.Add(numbers);
                                                        continue;
                                                    }
                                                    string text6 = Regex.Replace(xmlNode2.Attributes["DogumYeri"].Value, "\\s+", "");
                                                    if (text6 == "" || text6 == null)
                                                    {
                                                        text6 = text5;
                                                    }
                                                    string cinsiyet = Regex.Replace(xmlNode2.Attributes["Cinsiyet"].Value, "\\s+", "");
                                                    if (cinsiyet == "K") cinsiyet = "Kadın";
                                                    if (cinsiyet == "E") cinsiyet = "Erkek";



                                                    guest3.ID = text4;
                                                    guest3.NATION = Array.IndexOf<string>(nations, xmlNode2.Attributes["Uyrugu"].Value);
                                                    guest3.COUNTRY = text6;
                                                    guest3.NAME = xmlNode2.Attributes["Adi"].Value.ToString();
                                                    guest3.SURNAME = xmlNode2.Attributes["Soyadi"].Value.ToString();
                                                    guest3.BIRTH = string.Concat(new string[]
                                                                                                        {
                                                        xmlNode2.Attributes["DogumTarihi"].Value.Substring(8, 2),
                                                        ".",
                                                        xmlNode2.Attributes["DogumTarihi"].Value.Substring(5, 2),
                                                        ".",
                                                        xmlNode2.Attributes["DogumTarihi"].Value.Substring(0, 4)
                                                                                                        }).ToString();
                                                    guest3.FATHER = xmlNode2.Attributes["AnaAdi"].Value.ToString();
                                                    guest3.MOTHER = xmlNode2.Attributes["BabaAdi"].Value.ToString();
                                                    guest3.GENDER = Array.IndexOf<string>(genders, cinsiyet);
                                                    guest3.setForeign();

                                                }


                                                if (list.Where(x => x.ROOM == guest3.ROOM).Count() == 0)
                                                {
                                                    guest3.DETAIL = false;
                                                }
                                                else
                                                {
                                                    guest3.DETAIL = true;
                                                }
                                                list.Add(guest3);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    num++;
                                    list2.Add(numbers);
                                }
                            }
                        }

                        Console.WriteLine("XML Toplam kayıt sayısı : " + num2);
                        Console.WriteLine("XML Hatalı kayıt sayısı : " + num);
                        Console.WriteLine("XML Gecerli kayıt sayısı : " + list.Count);
                        KBS.connect(KBS_username, KBS_password, kod);
                        List<Guest> list3 = KBS.guests();
                        Console.WriteLine("KBS toplam kayıt sayısı : " + list3.Count);
                        using (List<Guest>.Enumerator enumerator2 = list.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Guest guest = enumerator2.Current;
                                List<Guest> list4 = list3.FindAll((Guest item) => string.Equals(item.ID, guest.ID, StringComparison.OrdinalIgnoreCase) && string.Equals(item.ROOM.ToString(), guest.ROOM.ToString(), StringComparison.OrdinalIgnoreCase));
                                if (list4.Count > 1)
                                {
                                    int num3 = 0;
                                    foreach (Guest current in list4)
                                    {
                                        if (num3 == 0)
                                        {
                                            num3++;
                                        }
                                        else
                                        {
                                            KBS.checkout(current.KEY);
                                            Console.WriteLine(current.KEY + " silindi " + current.ROOM);
                                            //AutoClosingMessageBox.Show(current.KEY + " silindi " + current.ROOM, "", 100);
                                        }
                                    }


                                }
                            }
                        }
                        using (List<Guest>.Enumerator enumerator2 = list3.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Guest guest = enumerator2.Current;
                                Guest guest2 = list.Find((Guest item) => string.Equals(item.ID, guest.ID, StringComparison.OrdinalIgnoreCase) && string.Equals(item.ROOM.ToString(), guest.ROOM.ToString(), StringComparison.OrdinalIgnoreCase));
                                if (guest2 == null)
                                {
                                    KBS.checkout(guest.KEY);
                                    Console.WriteLine(guest.KEY + " silindi " + guest.ROOM);
                                    //AutoClosingMessageBox.Show(guest.KEY + " silindi " + guest.ROOM, "", 100);
                                }
                            }
                        }
                        using (List<Guest>.Enumerator enumerator2 = list.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Guest guest = enumerator2.Current;
                                Guest guest2 = list3.Find((Guest item) => string.Equals(item.ID, guest.ID, StringComparison.OrdinalIgnoreCase) && string.Equals(item.ROOM.ToString(), guest.ROOM.ToString(), StringComparison.OrdinalIgnoreCase));
                                if (guest2 == null)
                                {
                                    if (guest.isTC())
                                    {
                                        KBS.addTCGuest(guest.ID, guest.ROOM, guest.PLATE, guest.DETAIL);
                                        Console.WriteLine(guest.ID + " (TC) eklendi " + guest.ROOM + " " + guest.NAME + " " + guest.SURNAME);
                                        //AutoClosingMessageBox.Show(guest.ID + " (TC) eklendi " + guest.ROOM, "", 100);
                                    }
                                    else
                                    {
                                        KBS.addForeignGuest(guest.ID, guest.NAME, guest.SURNAME, guest.BIRTH, guest.COUNTRY, guest.NATION.ToString(), guest.ROOM, guest.PLATE, guest.FATHER, guest.MOTHER, guest.GENDER.ToString(), guest.DETAIL);
                                        Console.WriteLine(guest.ID + " (Pasaport) eklendi " + guest.ROOM + " " + guest.NAME + " " + guest.SURNAME);
                                        //AutoClosingMessageBox.Show(guest.ID + " (Pasaport) eklendi " + guest.ROOM, "", 100);
                                    }
                                }
                            }
                        }
                        Console.WriteLine(KBS_Path + " basarılı.");

                        //                        List<Guest> listSon = KBS.guests();
                        //                        foreach (var item in listSon)
                        //                        {
                        //                            if (item.isTC())
                        //                            {
                        //                                SqlConnection con = new SqlConnection(RMOS_CONNSTR);
                        //                                SqlCommand cmd = new SqlCommand();
                        //                                cmd.Connection = con;
                        //                                cmd.CommandText = @"
                        //update Previl set 
                        //Kimlik_Ad = @Kimlik_Ad, 
                        //Kimlik_Soyad = @Kimlik_Soyad, 
                        //Kimlik_Baba = @Kimlik_Baba, 
                        //Kimlik_Ana = @Kimlik_Ana, 
                        //Kimlik_Dogum = @Kimlik_Dogum 
                        //where Kimlik_No = @Kimlik_No";
                        //                                cmd.Parameters.AddWithValue("@Kimlik_Ad", item.NAME);
                        //                                cmd.Parameters.AddWithValue("@Kimlik_Soyad", item.SURNAME);
                        //                                cmd.Parameters.AddWithValue("@Kimlik_Baba", item.FATHER);
                        //                                cmd.Parameters.AddWithValue("@Kimlik_Ana", item.MOTHER);
                        //                                cmd.Parameters.AddWithValue("@Kimlik_Dogum", item.BIRTH);
                        //                                cmd.Parameters.AddWithValue("@Kimlik_No", item.ID);
                        //                                if (con.State != System.Data.ConnectionState.Open) con.Open();
                        //                                cmd.ExecuteNonQuery();
                        //                                if(con.State != System.Data.ConnectionState.Closed) con.Close();


                        //                            }
                        //                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Geçerli bir xml dosyası degil.");
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Dosya Bulunamadı");
                }
            }
            finally
            {

            }
        }
    }
}
