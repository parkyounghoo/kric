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
    class subwayExtInfo
    {
        public void getsubwayExtInfo()
        {
            DataSet ds = Program.selectDS("select RAIL_OPR_ISTT_CD,LN_CD,STIN_CD,STIN_NM from subway_code");

            int time = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                time++;
                if (time == 40)
                {
                    time = 0;
                    Console.WriteLine("대기중 ....");
                    Thread.Sleep(5000);
                }
                DataRow dr = ds.Tables[0].Rows[i];

                string url = "http://openapi.kric.go.kr/openapi/convenientInfo/stationGateInfo?" +
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
                                   select new subway_ext_info
                                   {
                                       railOprIsttCd = r.Element("railOprIsttCd") == null ? "" : r.Element("railOprIsttCd").Value,
                                       lnCd = r.Element("lnCd") == null ? "" : r.Element("lnCd").Value,
                                       stinCd = r.Element("stinCd") == null ? "" : r.Element("stinCd").Value,
                                       exitNo = r.Element("exitNo") == null ? "" : r.Element("exitNo").Value,
                                       impFaclNm = r.Element("impFaclNm") == null ? "" : r.Element("impFaclNm").Value,
                                       dst = r.Element("dst") == null ? "" : r.Element("dst").Value,
                                       adr = r.Element("adr") == null ? "" : r.Element("adr").Value,
                                       telNo = r.Element("telNo") == null ? "" : r.Element("telNo").Value
                                   };

                    for (int j = 0; j < itemList.ToList().Count; j++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" insert into subway_ext_info values(");
                        sb.Append(" '" + itemList.ToList()[j].railOprIsttCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].lnCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].exitNo.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].impFaclNm.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].dst.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].adr.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].telNo.Replace("'", "") + "')");

                        Program.insert(sb.ToString());
                    }
                }
            }
        }
    }

    internal class subway_ext_info
    {
        public string railOprIsttCd { get; set; }
        public string lnCd { get; set; }
        public string stinCd { get; set; }
        public string exitNo { get; set; }
        public string impFaclNm { get; set; }
        public string dst { get; set; }
        public string adr { get; set; }
        public string telNo { get; set; }
    }
}
