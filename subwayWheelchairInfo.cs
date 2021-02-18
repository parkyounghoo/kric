using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace kric
{
    class subwayWheelchairInfo
    {
        public void getsubwayWheelchairInfo()
        {
            DataSet ds = Program.selectDS("select RAIL_OPR_ISTT_CD,LN_CD,STIN_CD,STIN_NM from subway_code");

            int time = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine("휠체어리프트 정보 : " + (i + 1) + " 번째");

                time++;
                if (time == 40)
                {
                    time = 0;
                    Console.WriteLine("대기중 ....");
                    Thread.Sleep(5000);
                }
                DataRow dr = ds.Tables[0].Rows[i];

                string url = "http://openapi.kric.go.kr/openapi/vulnerableUserInfo/stationWheelchairLiftMovement?" +
                    "serviceKey=$2a$10$wIALWEmYjhngdr6ufWWlauasON01Ma01bKNUtVpUH4ZpoCuWSo8SS" +
                    "&format=xml" +
                    "&railOprIsttCd=" + dr["RAIL_OPR_ISTT_CD"] +
                    "&lnCd=" + dr["LN_CD"] +
                    "&stinCd=" + dr["STIN_CD"];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = "";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();

                    XDocument doc = XDocument.Parse(results);

                    var itemList = from r in doc.Descendants("item")
                                   select new subway_wheelchair_info
                                   {
                                       railOprIsttCd = r.Element("railOprIsttCd") == null ? "" : r.Element("railOprIsttCd").Value,
                                       lnCd = r.Element("lnCd") == null ? "" : r.Element("lnCd").Value,
                                       stinCd = r.Element("stinCd") == null ? "" : r.Element("stinCd").Value,
                                       mvPathMgNo = r.Element("mvPathMgNo") == null ? "" : r.Element("mvPathMgNo").Value,
                                       mvPathDvCd = r.Element("mvPathDvCd") == null ? "" : r.Element("mvPathDvCd").Value,
                                       mvPathDvNm = r.Element("mvPathDvNm") == null ? "" : r.Element("mvPathDvNm").Value,
                                       mvTpOrdr = r.Element("mvTpOrdr") == null ? "" : r.Element("mvTpOrdr").Value,
                                       mvDst = r.Element("mvDst") == null ? "" : r.Element("mvDst").Value,
                                       mvContDtl = r.Element("mvContDtl") == null ? "" : r.Element("mvContDtl").Value
                                   };

                    for (int j = 0; j < itemList.ToList().Count; j++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" insert into kric_wheelchairInfo values(");
                        sb.Append(" '" + itemList.ToList()[j].railOprIsttCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].lnCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvPathMgNo.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvPathDvCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvPathDvNm.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvTpOrdr.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvDst.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mvContDtl.Replace("'", "") + "',");
                        sb.Append(" getdate() )");

                        Program.insert(sb.ToString());
                    }
                }
            }
        }
    }

    internal class subway_wheelchair_info
    {
        public string railOprIsttCd { get; set; }
        public string lnCd { get; set; }
        public string stinCd { get; set; }
        public string mvPathMgNo { get; set; }
        public string mvPathDvCd { get; set; }
        public string mvPathDvNm { get; set; }
        public string mvTpOrdr { get; set; }
        public string mvDst { get; set; }
        public string mvContDtl { get; set; }
    }
}
