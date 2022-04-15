using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace OrderShippingFunc
{
    public static class OrderDetails
    {
        [FunctionName("OrderDetails")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Get the customer's last name from the query string or the request body
            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            Order requestedOrder = await getOrderAsync(name);

            if (requestedOrder != null)
            {
                string orderJson = JsonConvert.SerializeObject(requestedOrder);
                return new HttpResponseMessage(HttpStatusCode.OK){
                    Content = new StringContent(orderJson, Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound){
                    Content = new StringContent("No order for that has been found for your request.")
                };
            }
        }

        private static async Task<Order> getOrderAsync(string customerLastName)
        {
            //This method simulates returning an order,
            //e.g. from a database
            // Get the connection string from app settings and use it to create a connection.
    var str = Environment.GetEnvironmentVariable("sqldb_connection");
     Order returnedOrder = new Order();
           
           
             SqlConnection conn = new SqlConnection("Server=tcp:dougsqlserverproject.database.windows.net,1433;Initial Catalog=DougProjectSQLDB;Persist Security Info=False;User ID=dougproject;Password=Sqlserver1404!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                conn.Open();
                var text = "SELECT [FirstName]  FROM [SalesLT].[Customer] where CustomerId=" + customerLastName  ;

                SqlCommand cmd = new SqlCommand(text, conn);
             using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                    if (reader.Read())
                    {
                      
                    returnedOrder.FirstName = String.Format("{0}",reader["FirstName"]);
                        
                    }
                    }

           conn.Close();
            
            

         

            return returnedOrder;
        }
    }

    public class Order 
    {
        public string FirstName { get; set; }
      
    }
}
