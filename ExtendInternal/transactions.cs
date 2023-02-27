using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

using DataModel;

namespace ExtendApi {
    class TransactionContext {

        private String auth_token;
        private readonly HttpClient http_client;        
        private readonly String get_virtual_card_transactions_api;
        private readonly String get_transaction_api;

        public TransactionContext(String auth_token_in, IConfiguration connection_strings, HttpClient http_client_in) {
            auth_token = auth_token_in;
            http_client = http_client_in;
            get_virtual_card_transactions_api = connection_strings["GetVirtualCardTransactions"] ?? "";
            get_transaction_api = connection_strings["GetTransaction"] ?? "";
        }

        // List the transactions associated with your virtual card.   
        public async Task<List<Transaction>> GetVirtualCardTransactions(String card_id) {
            String formatted_endpoint = String.Format(get_virtual_card_transactions_api, card_id);
            formatted_endpoint+="?status=PENDING,CLEARED,DECLINED";

            var request = new HttpRequestMessage(HttpMethod.Get, formatted_endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.paywithextend.v2021-03-12+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

            // Request and Response
            using ( HttpResponseMessage response = await http_client.SendAsync(request) ) {
                response.EnsureSuccessStatusCode(); // Throws an Error if the Req Fails
                
                var response_data = JsonSerializer.Deserialize<GetVirtualCardTransactionsResponse>(
                    await response.Content.ReadAsStringAsync());

                return response_data.transactions;
            }
        }
        // View the details for each individual transaction you’ve made.
        public async  Task<Transaction> GetTransaction(String tx_id) {
            String formatted_endpoint = String.Format(get_transaction_api, tx_id);

            var request = new HttpRequestMessage(HttpMethod.Get, formatted_endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.paywithextend.v2021-03-12+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth_token);

            // Request and Response
            using ( HttpResponseMessage response = await http_client.SendAsync(request) ) {
                response.EnsureSuccessStatusCode(); // Throws an Error if the Req Fails
                
                var response_data = JsonSerializer.Deserialize<Transaction>(
                    await response.Content.ReadAsStringAsync());

                return response_data;
            }

        }
    }
}

namespace DataModel {
    public class GetVirtualCardTransactionsResponse {
        public List<Transaction>? transactions { get; set;}
    }

    public class Transaction {
        public String? id {get;set;}
        public String? cardholderName {get;set;}
        public String? nameOnCard {get; set;}
        public String? status {get;set;}
        public int? authBillingAmountCents {get;set;}
        public String? authBillingCurrency {get;set;}
        public String? merchantName {get;set;}
        public String? mccGroup {get;set;}
        public String? authedAt {get;set;}
    }
}