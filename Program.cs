using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kric
{
    class Program
    {
        static string connectionString = "server = localhost; uid = sa; pwd = 1111; database = PrivateData;";
        static void Main(string[] args)
        {
            insert("truncate table kric_subwayInfo");
            insert("truncate table kric_cnvncInfo");
            insert("truncate table kric_wheelchairInfo");
            //역사별정보
            subwayInfo subwayInfo = new subwayInfo();
            subwayInfo.getsubwayInfo();

            //편의정보
            subwayCnvncInfo cnvncInfo = new subwayCnvncInfo();
            cnvncInfo.getsubwayCnvncInfo();

            //역사별 휠체어리프트 이동동선
            subwayWheelchairInfo wheelchairInfo = new subwayWheelchairInfo();
            wheelchairInfo.getsubwayWheelchairInfo();

            //subwayExtInfo extInfo = new subwayExtInfo();
            //extInfo.getsubwayExtInfo();
        }

        public static void insert(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandTimeout = 0; // timeout
                cmd.ExecuteNonQuery();
            }
        }

        public static DataSet selectDS(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
                _SqlDataAdapter.SelectCommand = new SqlCommand(query, conn);
                _SqlDataAdapter.Fill(ds);

                return ds;
            }
        }
    }
}
