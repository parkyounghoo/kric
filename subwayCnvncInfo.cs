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
    class subwayCnvncInfo
    {
        public void getsubwayCnvncInfo()
        {
            DataSet ds = Program.selectDS("select RAIL_OPR_ISTT_CD,LN_CD,STIN_CD,STIN_NM from subway_code");

            int time = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine("편의정보 : " + (i + 1) + " 번째");
                time++;
                if (time == 40)
                {
                    time = 0;
                    Console.WriteLine("대기중 ....");
                    Thread.Sleep(5000);
                }
                DataRow dr = ds.Tables[0].Rows[i];

                string url = "http://openapi.kric.go.kr/openapi/convenientInfo/stationCnvFacl?" +
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
                                   select new subway_cnvnc_info
                                   {
                                       lnCd = dr["LN_CD"].ToString(),
                                       railOprIsttCd = dr["RAIL_OPR_ISTT_CD"].ToString(),
                                       stinCd = dr["STIN_CD"].ToString(),
                                       gubun = r.Element("gubun") == null ? "" : r.Element("gubun").Value,
                                       grndDvCd = r.Element("grndDvCd") == null ? "" : r.Element("grndDvCd").Value,
                                       stinFlor = r.Element("stinFlor") == null ? "" : r.Element("stinFlor").Value,
                                       dtlLoc = r.Element("dtlLoc") == null ? "" : r.Element("dtlLoc").Value,
                                       trfcWeakDvCd = r.Element("trfcWeakDvCd") == null ? "" : r.Element("trfcWeakDvCd").Value,
                                       mlFmlDvCd = r.Element("mlFmlDvCd") == null ? "" : r.Element("mlFmlDvCd").Value,
                                       imgPath = r.Element("imgPath") == null ? "" : r.Element("imgPath").Value
                                   };

                    for (int j = 0; j < itemList.ToList().Count; j++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" insert into kric_cnvncInfo values(");
                        sb.Append(" '" + itemList.ToList()[j].railOprIsttCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].lnCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].gubun.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].grndDvCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinFlor.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].dtlLoc.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].trfcWeakDvCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mlFmlDvCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].imgPath.Replace("'", "") + "',");
                        sb.Append(" getdate() )");

                        Program.insert(sb.ToString());
                    }
                }
            }
        }
    }

    internal class subway_cnvnc_info
    {
        public string lnCd { get; set; }
        public string railOprIsttCd { get; set; }
        public string stinCd { get; set; }
        public string gubun { get; set; }
        public string grndDvCd { get; set; }
        public string stinFlor { get; set; }
        public string dtlLoc { get; set; }
        public string trfcWeakDvCd { get; set; }
        public string mlFmlDvCd { get; set; }
        public string imgPath { get; set; }
    }
}
