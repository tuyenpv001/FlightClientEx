// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License.  You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;
using Apache.Arrow.Flight.Client;
using Apache.Arrow.Flight;
using Apache.Arrow;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Http;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authentication;
using System.Text;

namespace FlightClientExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Tạo một đối tượng Dictionary để chứa các thuộc tính
            Dictionary<string, string> props = new Dictionary<string, string>();

            props.Add(UtilAbstract.DOMAIN_SERVER, "172.16.2.138");
            props.Add(UtilAbstract.DOMAIN_PORT, "32010");
            props.Add(UtilAbstract.CLIENT_APIKEY, "034.01.01.M01");
            props.Add(UtilAbstract.CLIENT_SECRET, "63a404b44ec51a56cc641f68");
            props.Add(UtilAbstract.CLIENT_ID, "034.01.01.M01");


            string host = !String.IsNullOrEmpty(props[UtilAbstract.DOMAIN_SERVER]) ? props[UtilAbstract.DOMAIN_SERVER] : "172.16.2.138";
            string port = !String.IsNullOrEmpty(props[UtilAbstract.DOMAIN_PORT]) ? props[UtilAbstract.DOMAIN_PORT] : "32010";
            string password = $"{props[UtilAbstract.CLIENT_APIKEY]}@{props[UtilAbstract.CLIENT_ID]}";
            // Create client
            // (In production systems, you should use https not http)
            var address = $"https://{host}:{port}";
            Console.WriteLine($"Connecting to: {address}");
            //string token = await GetAccessToken(props[UtilAbstract.CLIENT_SECRET], password, props[UtilAbstract.CLIENT_ID], address, props[UtilAbstract.CLIENT_SECRET] );
            //Console.WriteLine($"Access Token: {token}");
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(props[UtilAbstract.CLIENT_SECRET] + ":" + password));
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("Authorization", "Basic " + encoded);
                return Task.CompletedTask;
            });
     
           
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpHandler = httpHandler,
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            //var channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);


            FlightClient client = new FlightClient(channel);
            //client.DoAction
            var credentialCallOption = await Authenticate(client, password, props[UtilAbstract.CLIENT_SECRET]);
            var tokkkkk = AuthenticateAndGetToken(client, password, props[UtilAbstract.CLIENT_SECRET], address);
            var actions = client.ListActions();
            while (await actions.ResponseStream.MoveNext(default))
            {
                Console.WriteLine(actions.ResponseStream.Current);

            }
            // Tạo Metadata chứa thông tin xác thực
            //Metadata headers = new Metadata();
            //headers.Add("Authorization", $"Bearer {authToken}");
            // Tạo kênh gRPC với thông tin xác thực
            //var channel = GrpcChannel.ForAddress(address) ;
            //var client = new FlightClient(channel);

            var recordBatches = new RecordBatch[] {
                CreateTestBatch(0, 2000), CreateTestBatch(50, 9000)
            };

            //// Particular flights are identified by a descriptor. This might be a name,
            //// a SQL query, or a path. Here, just using the name "test".
            var descriptor = FlightDescriptor.CreateCommandDescriptor("SELECT * FROM cloudquery.review_category.time_zone_test WHERE time = 1136048400000 LIMIT 10");

            //// Upload data with StartPut
            var batchStreamingCall = client.StartPut(descriptor);
            foreach (var batch in recordBatches)
            {
                await batchStreamingCall.RequestStream.WriteAsync(batch);
            }
            //// Signal we are done sending record batches
            //await batchStreamingCall.RequestStream.CompleteAsync();
            //// Retrieve final response
            //await batchStreamingCall.ResponseStream.MoveNext();
            //Console.WriteLine(batchStreamingCall.ResponseStream.Current.ApplicationMetadata.ToStringUtf8());
            //Console.WriteLine($"Wrote {recordBatches.Length} batches to server.");

            //// Request information:
            //var schema = await client.GetSchema(descriptor).ResponseAsync;
            //Console.WriteLine($"Schema saved as: \n {schema}");

            //var info = await client.GetInfo(descriptor).ResponseAsync;
            //Console.WriteLine($"Info provided: \n {info}");

            //Console.WriteLine($"Available flights:");
            //var flights_call = client.ListFlights();

            //while (await flights_call.ResponseStream.MoveNext())
            //{
            //    Console.WriteLine("  " + flights_call.ResponseStream.Current.ToString());
            //}

            //// Download data
            //await foreach (var batch in StreamRecordBatches(info))
            //{
            //    Console.WriteLine($"Read batch from flight server: \n {batch}");
            //}

            //// See available commands on this server
            //var action_stream = client.ListActions();
            //Console.WriteLine("Actions:");
            //while (await action_stream.ResponseStream.MoveNext())
            //{
            //    var action = action_stream.ResponseStream.Current;
            //    Console.WriteLine($"  {action.Type}: {action.Description}");
            //}

            //// Send clear command to drop all data from the server.
            //var clear_result = client.DoAction(new FlightAction("clear"));
            //await clear_result.ResponseStream.MoveNext(default);
        }

        //public static async IAsyncEnumerable<RecordBatch> StreamRecordBatches(
        //    FlightInfo info
        //)
        //{
        // There might be multiple endpoints hosting part of the data. In simple services,
        // the only endpoint might be the same server we initially queried.
        //foreach (var endpoint in info.Endpoints)
        //{
        //    // We may have multiple locations to choose from. Here we choose the first.
        //    var download_channel = GrpcChannel.ForAddress(endpoint.Locations.First().Uri);
        //    var download_client = new FlightClient(download_channel);

        //    var stream = download_client.GetStream(endpoint.Ticket);

        //    while (await stream.ResponseStream.MoveNext())
        //    { 
        //        yield return stream.ResponseStream.Current;
        //    }
        //}
        //}

        //static CredentialCallOption Authenticate(FlightClient client, string user, string pass)
        //{
        //    // Tạo danh sách CallOption
        //    var callOptions = new List<CallOptions>();

        //    // Thêm CredentialCallOption cho xác thực.
        //    // Một CredentialCallOption được khởi tạo với một instance của
        //    // BasicAuthCredentialWriter.
        //    // BasicAuthCredentialWriter nhận cặp tên người dùng và mật khẩu, mã hóa
        //    // cặp đó và
        //    // chèn thông tin xác thực vào tiêu đề Authorization để xác thực với máy chủ.
        //    callOptions.Add(new CredentialCallOption(new BasicAuthCredentialWriter(user, pass)));

        //    // Thực hiện handshake với Flight Server Endpoint.
        //    client.Handshake(callOptions.ToArray());

        //    // Xác thực thành công, trích xuất token bearer được trả về bởi máy chủ
        //    // từ Factory.
        //    // CredentialCallOption có thể được sử dụng trong các cuộc gọi RPC Flight sau
        //    // này cho xác thực token bearer.
        //    return factory.GetCredentialCallOption();
        //}

        static async Task<CredentialCallOptions> Authenticate(FlightClient client, string user, string pass)
        {
            // Tạo instance của BasicAuthCredentialWriter với tên người dùng và mật khẩu
            var credentialWriter = new BasicAuthCredentialWriter(user, pass);

            // Lấy metadata từ BasicAuthCredentialWriter
            var metadata = await credentialWriter.GetMetadataAsync();

            var res = client.Handshake(metadata);

            Console.WriteLine(res);
            // Trả về CredentialCallOptions với metadata
            return new CredentialCallOptions(metadata);
        }


        static async Task<string> GetAccessToken(string username, string password, string clientId, string tokenEndpoint, string clientSecret)
        {
            // Set up HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Define token endpoint and client credentials
         
         

                // Create token request
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                tokenRequest.Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                // You might need to include additional parameters based on your authentication server configuration
            });

                // Send token request
                var response = await client.SendAsync(tokenRequest);

                // Read token response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize JSON response to extract access token
                    // Note: Your response structure may vary depending on your authentication server
                    dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                    string accessToken = responseData.access_token;
                    return accessToken;
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve access token. Status code: {response.StatusCode}");
                }
            }
        }


        static async Task<string> AuthenticateAndGetToken(FlightClient client, string user, string pass, string url)
        {
            //// Tạo metadata với thông tin xác thực
            //var metadata = new Metadata();
            //var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user}:{pass}"));
            //metadata.Add("Authorization", "Basic " + encoded);

            //// Tạo call options với metadata
            //var callOptions = new CallOptions(metadata);

            //// Thực hiện handshake với máy chủ Flight
            //client.Handshake(callOptions.Headers);

            //// Lấy thông tin xác thực từ metadata
            //string token = null;
            //if (metadata.Count > 0)
            //{

            //    // Trả về token
            //    return Task.FromResult(token);
            //}


            //// Trả về token
            //return Task.FromResult(token);

            var client1 = new HttpClient();
            client1.BaseAddress = new Uri(url);
            client1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{pass}")));
            var response = await client1.GetAsync("/");

            return "";
        }


        public static RecordBatch CreateTestBatch(int start, int length)
        {
            return new RecordBatch.Builder()
                .Append("Column A", false, col => col.Int32(array => array.AppendRange(Enumerable.Range(start, start + length))))
                .Append("Column B", false, col => col.Float(array => array.AppendRange(Enumerable.Range(start, start + length).Select(x => Convert.ToSingle(x * 2)))))
                .Append("Column C", false, col => col.String(array => array.AppendRange(Enumerable.Range(start, start + length).Select(x => $"Item {x+1}"))))
                .Append("Column D", false, col => col.Boolean(array => array.AppendRange(Enumerable.Range(start, start + length).Select(x => x % 2 == 0))))
                .Build();
        }
    }


    public class CredentialCallOptions
    {
        public Metadata Metadata { get; set; }

        public CredentialCallOptions(Metadata metadata)
        {
            Metadata = metadata;
        }
    }

    public class BasicAuthCredentialWriter
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthCredentialWriter(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public Task<Metadata> GetMetadataAsync()
        {
            var metadata = new Metadata();
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
            metadata.Add("Authorization", "Basic " + encoded);
            return Task.FromResult(metadata);
        }
    }
}
