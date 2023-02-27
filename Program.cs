using Microsoft.Extensions.Configuration;
using System.ComponentModel;

using DataModel;


namespace MyApplication
{
    // https://medium.com/@dozieogbo/a-better-way-to-inject-appsettings-in-asp-net-core-96be36ffa22b
    class Program
    {
        static void Main(string[] args)
        {           
            // Init Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();

            // Debating use of Null Coalescing Operator, depending on application, but alternatives depend on
            // What the context of the application is.
            String email = config.GetSection("auth")["email"] ?? "" ; 
            String password = config.GetSection("auth")["password"] ?? ""; 

            // Init Singletons, Contexts, etc
            var client = new HttpClient();
            var session = new ExtendSession(email, password, config.GetSection("ConnectionStrings"), client);
            
     
            // "Application Logic" 
            Console.WriteLine("Hi, Welcome to the JenningsCo. Pay with Extend Terminal");
            Console.WriteLine("You are logging in as: " + email);
            
            while(true) {
                Console.Write("\nPlease Enter your Command: ");
                String cmd = Console.ReadLine().ToLower();
                if (cmd == "demo") {
                    Console.WriteLine("Fetching all Virtual Cards for the user...");
                    List<VirtualCard> all_user_cards = session.GetUserVirtualCards();
                    
                    Console.WriteLine("This user owns the following Cards: ");
                    foreach (VirtualCard card in all_user_cards) {
                        Console.WriteLine(String.Format(
                            "     Card Name: {0} | Current Balance: {1} | Limit: {2} | expires: {3}",
                            card.displayName, card.balanceCents, card.limitCents, card.expires));
                    }

                    Console.WriteLine("\nFor this Card, we will now list the Transaction History:");
                    List<Transaction> all_card_tx = session.GetAllCardTransactions(all_user_cards[0].id);
                    foreach (Transaction tx in all_card_tx) {
                        Console.WriteLine(String.Format(
                            "     Tx: From Merchant: {0} of amount: {1} {2} at {3}  ",
                            tx.merchantName, tx.authBillingCurrency, Convert.ToDouble(tx.authBillingAmountCents)/100,
                            tx.authedAt));
                    }

                    Console.WriteLine(
                        "\nWe will fetch a more detailed and technical transaction report for the first of the previous list:");
                    Transaction my_tx = session.GetTransactionDetails(all_card_tx[0].id);
                    PrintProperties(my_tx);
                }

                else if (cmd == "quit") {
                    Console.WriteLine("\nThanks for using the Terminal and We hope you have a nice day\n");
                    break;
                }
            }

        }

        // helper function to help display things
        public static void PrintProperties(object obj) {
            // Helper Function to 
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
    }
} // MyApplication