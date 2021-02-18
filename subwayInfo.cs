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
    class subwayInfo
    {
        public void getsubwayInfo()
        {
            DataSet ds = Program.selectDS("select RAIL_OPR_ISTT_CD,LN_CD,STIN_CD,STIN_NM from subway_code");

            int time = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine("역사별 정보 : " + (i + 1) + " 번째");
                time++;
                if (time == 40)
                {
                    time = 0;
                    Console.WriteLine("대기중 ....");
                    Thread.Sleep(5000);
                }
                DataRow dr = ds.Tables[0].Rows[i];

                string url = "http://openapi.kric.go.kr/openapi/convenientInfo/stationInfo?" +
                    "serviceKey=$2a$10$wIALWEmYjhngdr6ufWWlauasON01Ma01bKNUtVpUH4ZpoCuWSo8SS" +
                    "&format=xml" +
                    "&railOprIsttCd=" + dr["RAIL_OPR_ISTT_CD"] +
                    "&lnCd=" + dr["LN_CD"] +
                    "&stinCd=" + dr["STIN_CD"] +
                    "&stinNm=" + dr["STIN_NM"];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = "";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();

                    XDocument doc = XDocument.Parse(results);

                    var itemList = from r in doc.Descendants("item")
                                   select new subway_info
                                   {
                                       railOprIsttCd = r.Element("railOprIsttCd") == null ? "" : r.Element("railOprIsttCd").Value,
                                       lnCd = r.Element("lnCd") == null ? "" : r.Element("lnCd").Value,
                                       stinCd = r.Element("stinCd") == null ? "" : r.Element("stinCd").Value,
                                       stinNm = r.Element("stinNm") == null ? "" : r.Element("stinNm").Value,
                                       stinNmEng = r.Element("stinNmEng") == null ? "" : r.Element("stinNmEng").Value,
                                       lonmAdr = r.Element("lonmAdr") == null ? "" : r.Element("lonmAdr").Value,
                                       roadNmAdr = r.Element("roadNmAdr") == null ? "" : r.Element("roadNmAdr").Value,
                                       stinLocLon = r.Element("stinLocLon") == null ? "" : r.Element("stinLocLon").Value,
                                       stinLocLat = r.Element("stinLocLat") == null ? "" : r.Element("stinLocLat").Value,
                                       mapCordX = r.Element("mapCordX") == null ? "" : r.Element("mapCordX").Value,
                                       mapCordY = r.Element("mapCordY") == null ? "" : r.Element("mapCordY").Value,
                                       strkZone = r.Element("strkZone") == null ? "" : r.Element("strkZone").Value
                                   };

                    for (int j = 0; j < itemList.ToList().Count; j++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(" insert into kric_subwayInfo values(");
                        sb.Append(" '" + itemList.ToList()[j].railOprIsttCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].lnCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinCd.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinNm.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinNmEng.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].lonmAdr.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].roadNmAdr.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinLocLon.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].stinLocLat.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mapCordX.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].mapCordY.Replace("'", "") + "',");
                        sb.Append(" '" + itemList.ToList()[j].strkZone.Replace("'", "") + "',");
                        sb.Append(" getdate() )");

                        Program.insert(sb.ToString());
                    }
                }
            }
        }
    }

    internal class subway_info
    {
        public string railOprIsttCd { get; set; }
        public string lnCd { get; set; }
        public string stinCd { get; set; }
        public string stinNm { get; set; }
        public string stinNmEng { get; set; }
        public string lonmAdr { get; set; }
        public string roadNmAdr { get; set; }
        public string stinLocLon { get; set; }
        public string stinLocLat { get; set; }
        public string mapCordX { get; set; }
        public string mapCordY { get; set; }
        public string strkZone { get; set; }
    }
}
