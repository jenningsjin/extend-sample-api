using Microsoft.Extensions.Configuration;

using ExtendApi;
using DataModel;

using static ExtendApi.AuthContext;
using static Util.util;


namespace MyApplication {
    // Extend Session is intented to be the entry point into the Extend API for our application
    // 
    // Modern .NET conventions lean toward dependency injection for managing dependencies.
    // So that will inform our designs here.
    public class ExtendSession {
        private String auth_token;
        private IConfigurationSection connection_strings;
        private HttpClient http_client;

        public ExtendSession(String username, String password, IConfigurationSection connection_strings_in, HttpClient http_client_in) {
            var auth_context = new AuthContext(auth_token, connection_strings_in, http_client_in);
            
            auth_token = auth_context.SignIn(username, password).Result;
            http_client = http_client_in;
            connection_strings = connection_strings_in;
        }

        // .NET Core Prefers to minimize the number of clients
        // See: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines

        public  List<VirtualCard> GetUserVirtualCards() {
            var virtual_cards_context = new VirtualCardsContext(auth_token, connection_strings, http_client);
            return virtual_cards_context.GetUserVirtualCards().Result;
        }

        public List<Transaction> GetAllCardTransactions(String card_id) {
            var transaction_context = new TransactionContext(auth_token, connection_strings, http_client);
            return transaction_context.GetVirtualCardTransactions(card_id).Result;
        }

        public Transaction GetTransactionDetails(String tx_id) {
            var transaction_context = new TransactionContext(auth_token, connection_strings, http_client);
            return transaction_context.GetTransaction(tx_id).Result;

        }

    }
}