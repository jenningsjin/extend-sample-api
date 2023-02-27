using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

using Request;
using DataModel;

namespace ExtendApi {    
    public class AuthContext {
        private String auth_token;
        private readonly HttpClient http_client;
        private readonly String sign_in_api;

        public AuthContext(String auth_token_in, IConfiguration connection_strings, HttpClient http_client_in) {
            auth_token = auth_token_in;
            http_client = http_client_in;
            sign_in_api = connection_strings["SignIn"] ?? "";
        }

        public async Task<String> SignIn(string email, string password) {

            // Build The Request
            var request = new HttpRequestMessage(HttpMethod.Post, sign_in_api);
            SignInRequest request_body = new SignInRequest {
                email = email,
                password = password
            };
            request.Content = new StringContent(JsonSerializer.Serialize(request_body));
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.paywithextend.v2021-03-12+json"));

            using( HttpResponseMessage response = await http_client.SendAsync(request)) {;
                response.EnsureSuccessStatusCode(); // Throws an Error if the Req Fails
                var s = JsonSerializer.Deserialize<SignInResponseView>(await response.Content.ReadAsStringAsync()); // This maps values if detected        
                return s.token;
            }
        }
    }
}

namespace Request {
    public class SignInRequest {
        public String? email {get; set;}
        public String? password {get; set;}
    }
}

namespace DataModel {
    public class SignInResponseView {
        public String? token {get; set;}
    }
}
