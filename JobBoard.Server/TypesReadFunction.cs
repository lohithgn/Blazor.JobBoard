using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace JobBoard.Server
{
    public class TypesReadFunction : FunctionBase
    {
        
        [FunctionName(nameof(TypesReadFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "types")] HttpRequest req,
            [CosmosDB("%DatabaseName%",
                collectionName: "%ContainerName%",
                ConnectionStringSetting = "CosmosConnectionString")] DocumentClient client,
            ILogger log)
        {

            try
            {
                var sqlQuery = "SELECT DISTINCT VALUE c.Type FROM c ORDER BY c.Type";

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, ContainerName);

                var query = client.CreateDocumentQuery<string>(collectionUri, sqlQuery,new FeedOptions()
                {
                    EnableCrossPartitionQuery = true
                }).AsDocumentQuery();
                string[] types = {};
                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<string>();
                    types = response.ToArray();
                }

                return new OkObjectResult(types);
            }
            catch (Exception e)
            {
                return new ExceptionResult(e, false);
            }
        }
    }
}
