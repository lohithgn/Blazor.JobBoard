using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JobBoard.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace JobBoard.Server
{
    public class JobListingFunction : FunctionBase
    {
        
        [FunctionName(nameof(JobListingFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs")] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: "%ContainerName%",
                ConnectionStringSetting = "CosmosConnectionString")] DocumentClient client,
            ILogger log
            )
        {
            
            try
            {
                var rawQuery = "SELECT * FROM c ";
                var filters = new List<string>();
                if (req.Query.TryGetValue("locations", out StringValues locationsValue) && !string.IsNullOrEmpty(locationsValue))
                {
                    var locations = locationsValue.ToString().Split(',').Select(a => $"\"{a}\"");
                    filters.Add($"c.Location IN ({string.Join(",",locations)})");
                }

                if (req.Query.TryGetValue("types",out StringValues typesValue) && !string.IsNullOrEmpty(typesValue))
                {
                    var types = typesValue.ToString().Split(',').Select(a => $"\"{a}\"");;
                    filters.Add($"c.Type IN ({string.Join(",",types)})");
                }

                if (req.Query.TryGetValue("q", out StringValues searchString) && !string.IsNullOrEmpty(searchString))
                {
                    filters.Add($"CONTAINS(LOWER(c.Title),\"{searchString}\") OR CONTAINS(LOWER(c.Subtitle),\"{searchString}\")");
                }

                if (filters.Count > 0)
                {
                    rawQuery += " WHERE " + string.Join(" AND ", filters);
                }

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, ContainerName);

                var query = client.CreateDocumentQuery<Job>(collectionUri, rawQuery,new FeedOptions()
                    {
                        EnableCrossPartitionQuery = true
                    }).AsDocumentQuery();
                var jobs = new List<Job>();
                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<Job>();
                    jobs.AddRange(response);
                }
                return new OkObjectResult(jobs);
            }
            catch (Exception e)
            {
                return new ExceptionResult(e, false);
            }
        }
    }
}
