using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Arrow.Flight.Client;
using Apache.Arrow.Flight;
using Newtonsoft.Json.Linq;

namespace FlightClientExample
{
    public class SqlCommand : UtilAbstract
    {
        private static string CATEGORY_COMMAND = "CATEGORY";
        private static string DATASET_COMMAND = "DATASET";
        private static string TABLE_COMMAND = "TABLE";
        private static string QUERY_COMMAND = "QUERY";

        public SqlCommand(Dictionary<string, string> props) : base(props)
        {
        }


        //public List<JObject> Query(string sql)
        //{
        //    try
        //    {
        //        // Bắt đầu với tham số JSONObject
        //        JObject json = new JObject();

        //        // Loại lệnh cần thiết
        //        json[HUB_FACTORY] = QUERY_COMMAND;
        //        json[SERVICE_PARAM] = sql;

        //        return GetCmd(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý ngoại lệ và trả về danh sách trống
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        return new List<JObject>();
        //    }
        //}



        //public List<JObject> GetCmd(JObject json)
        //{
        //    FlightClient flight = null;
        //    FlightStream stream = null;
        //    List<JObject> items = new List<JObject>();

        //    try (var allocator = new RootAllocator())
        //{
        //        var factory = new ClientIncomingAuthHeaderMiddleware.Factory(new ClientBearerHeaderHandler());

        //        flight = GetFlight(allocator, factory);

        //        var callOption = new HeaderCallOption(new FlightCallHeaders());
        //        var bearerToken = GetToken(factory, callOption, flight);

        //        var command = new FlightDescriptor.Command(json.ToString().GetBytes());

        //        var flightInfo = flight.GetInfo(command, bearerToken, callOption);

        //        stream = flight.GetStream(flightInfo.Endpoints[0].Ticket, bearerToken, callOption);
        //        if (stream != null)
        //        {
        //            while (stream.Next())
        //            {
        //                using (var currentRoot = stream.GetRoot())
        //                {
        //                    // Bắt đầu đọc dữ liệu
        //                    var vectors = Reader(currentRoot);

        //                    try
        //                    {
        //                        for (int idx = 0; idx < currentRoot.RowCount; idx++)
        //                        {
        //                            var item = new JObject();

        //                            foreach (var r in vectors)
        //                            {
        //                                item[r.Name] = r.Reader(idx).ToString();
        //                            }

        //                            items.Add(item);
        //                        }
        //                    }
        //                    finally
        //                    {
        //                        CloseQuietly(vectors);
        //                    }
        //                }
        //            }
        //        }

        //        return items;
        //    }
        //finally
        //    {
        //        CloseQuietly(stream);
        //        CloseQuietly(flight);
        //    }
        //}

    }
}
