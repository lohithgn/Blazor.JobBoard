using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace JobBoard.Server
{
    public class LocationReadFunction : FunctionBase
    {
        [FunctionName(nameof(LocationReadFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "locations")] HttpRequest req,
            [CosmosDB("%DatabaseName%",
                collectionName: "%ContainerName%",
                ConnectionStringSetting = "CosmosConnectionString")] DocumentClient client,
            ILogger log)
        {
            try
            {

                var sqlQuery = "SELECT DISTINCT VALUE c.Location FROM c ORDER BY c.Location";

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, ContainerName);

                var query = client.CreateDocumentQuery<string>(collectionUri, sqlQuery,new FeedOptions()
                {
                    EnableCrossPartitionQuery = true
                }).AsDocumentQuery();

                string[] locations = {};
                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<string>();
                    locations = response.ToArray();
                }

                return new OkObjectResult(locations);
            }
            catch (Exception e)
            {
                return new ExceptionResult(e, false);
            }

        }
    }
}
