using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Arrow.Flight.Client;
using Grpc.Core;

namespace FlightClientExample
{
    public abstract class UtilAbstract
    {
        protected const string HUB_FACTORY = "datasshub";
        protected const string SERVICE_PARAM = "serviceID";

        public const string DOMAIN_SERVER = "server.domain";
        public const string DOMAIN_PORT = "server.port";
        public const string CLIENT_ID = "client.id";
        public const string CLIENT_APIKEY = "client.apikey";
        public const string CLIENT_SECRET = "client.secret";

        private readonly Dictionary<string,string> props;

        public UtilAbstract(Dictionary<string, string> props)
        {
            this.props = props;
        }

        private string Domain()
        {
            //return props.(DOMAIN_SERVER, "calista-dev.inetcloud.vn");
            return  !String.IsNullOrEmpty(props[DOMAIN_SERVER]) ? props[DOMAIN_SERVER] : "calista-dev.inetcloud.vn";
        }

        private int Port()
        {
            try
            {
                string port = !String.IsNullOrEmpty(props[DOMAIN_PORT]) ? props[DOMAIN_PORT] : "32011" ; 
                return int.Parse(port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 32011;
            }
        }

        private string ClientID()
        {
            return !String.IsNullOrEmpty(props[CLIENT_ID]) ? props[CLIENT_ID] : "";
        }

        private string ApiKey()
        {
            return !String.IsNullOrEmpty(props[CLIENT_APIKEY]) ? props[CLIENT_APIKEY] : "";
        }

        private string Secret()
        {
            return !String.IsNullOrEmpty(props[CLIENT_SECRET]) ? props[CLIENT_SECRET] : "";
        }

        protected void CloseQuietly(FlightClient flight)
        {
            if (flight != null)
            {
                try
                {
                   
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
        }

        //protected CredentialCallOption GetToken(ClientIncomingAuthHeaderMiddleware.Factory factory, HeaderCallOption callOption, FlightClient flight)
        //{
        //    // Lưu ý: APIKEY phải được bao gồm trong yêu cầu {apikey}"@"{owner organId}
        //    return string.IsNullOrWhiteSpace(ClientID()) ?
        //        Authenticate(flight, ApiKey(), Secret(), factory, callOption) :
        //        Authenticate(flight, $"{ApiKey()}@{ClientID()}", Secret(), factory, callOption);
        //}

        //private CredentialCallOption Authenticate(FlightClient client,
        //    string user,
        //    string pass,
        //    ClientIncomingAuthHeaderMiddleware.Factory factory,
        //    HeaderCallOption clientProperties)
        //{
        //    var callOptions = new List<CallOptions>();

        //    // Thêm CredentialCallOption cho xác thực.
        //    // CredentialCallOption được khởi tạo với một instance của BasicAuthCredentialWriter.
        //    // BasicAuthCredentialWriter nhận cặp tên người dùng và mật khẩu, mã hóa cặp này và
        //    // chèn các thông tin đăng nhập vào tiêu đề Authorization để xác thực với máy chủ.
        //    callOptions.Add(new CredentialCallOptions(new BasicAuthCredentialWriter(user, pass)));

        //    // Nếu được cung cấp, thêm các thuộc tính của client vào CallOptions.
        //    if (clientProperties != null)
        //    {
        //        callOptions.Add(clientProperties);
        //    }

        //    // Thực hiện handshake với Endpoint máy chủ Arrow Flight.
        //    client.Handshake(callOptions.ToArray());

        //    // Xác thực thành công, trích xuất mã thông báo trả về từ máy chủ
        //    // từ ClientIncomingAuthHeaderMiddleware.Factory. CredentialCallOption có thể được
        //    // sử dụng trong các yêu cầu RPC tiếp theo cho xác thực token.
        //    return factory.GetCredentialCallOption();
        //}
    }
}
