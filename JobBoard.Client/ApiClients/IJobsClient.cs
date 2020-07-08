using System.Collections.Generic;
using System.Threading.Tasks;
using JobBoard.Client.Models;
using JobBoard.Shared.Models;
using Refit;

namespace JobBoard.Client.ApiClients
{
    public interface IJobsClient
    {
        [Get("/api/locations")]
        Task<string[]> GetLocations();

        [Get("/api/types")]
        Task<string[]> GetTypes();

        [Get("/api/jobs")]
        Task<IReadOnlyList<Job>> GetJobs(JobSearchParams jobSearchParams);
    }
}
