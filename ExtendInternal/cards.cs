using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

using Request;
using DataModel;

namespace ExtendApi {
    class VirtualCardsContext {

        private String auth_token;
        private readonly HttpClient http_client;
        private readonly String get_virtual_cards_api;
        

        public VirtualCardsContext(String auth_token_in, IConfiguration connection_strings, HttpClient http_client_in) {
            get_virtual_cards_api = connection_strings["GetUserVirtualCards"] ?? "";
            auth_token = auth_token_in;
            http_client = http_client_in;
        }

        // See: https://developer.paywithextend.com/docs/developer-paywithextend/3d09ea7e580e6-get-user-virtual-cards
        public async Task<List<VirtualCard>> GetUserVirtualCards() {
            var request = new HttpRequestMessage(HttpMethod.Get, get_virtual_cards_api);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.paywithextend.v2021-03-12+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

            using (HttpResponseMessage response = await http_client.SendAsync(request)) {;
                response.EnsureSuccessStatusCode(); // Throws an Error if the Req Fails
                
                string response_body = await response.Content.ReadAsStringAsync();

                var s = JsonSerializer.Deserialize<GetUserVirtualCardsResponse>(response_body); // This maps values if detected
                // PrintPropreties(s.virtualCards[0]);
                return s.virtualCards;
            }
        }
    }
}

namespace DataModel {
    public class GetUserVirtualCardsResponse {
        public List<VirtualCard>? virtualCards {get; set;}
    }

    public class VirtualCard {
        public String? id {get; set;}
        public String? displayName {get; set;}

        public int? limitCents {get; set;}
        public int? balanceCents {get; set;}
        public String? currency {get; set;}
        public String? expires {get; set;}
    }
}