using System.Collections.Generic;
using System.Threading.Tasks;
using JobBoard.Client.ApiClients;
using JobBoard.Client.Models;
using JobBoard.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace JobBoard.Client.Pages
{
    public partial class Index
    {
        [Inject]
        private IJobsClient Client { get; set; }

        [Inject]
        private DialogService DialogService { get; set; }

        private IReadOnlyList<Job> jobs; 
        private string[] locations; 
        private string[] types;
        private bool isBusy;
        private bool isJobsLoading;
        private IEnumerable<string> selectedLocations=new string[] {};
        private IEnumerable<string> selectedTypes = new string[] {};
        private string searchString;
        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            locations = await Client.GetLocations();
            types = await Client.GetTypes();
            jobs = await Client.GetJobs(new JobSearchParams());
            isBusy = false;
        }

        void OnSearchQueryChange(string value)
        {
            searchString = value;
        }

        async Task SearchJobs()
        {
            await JobFilterChanged();
        }

        async Task JobFilterChanged()
        {
            var jobParams = new JobSearchParams()
            {
                SearchString = searchString,
                Locations = selectedLocations == null ? "" : string.Join(",", selectedLocations),
                Types = selectedTypes == null ? "" : string.Join(",", selectedTypes),
            };
            isJobsLoading = true;            
            jobs = await Client.GetJobs(jobParams);
            isJobsLoading = false;            
        }

        async Task ShowJob(Job job)
        {
            await DialogService.OpenAsync<JobDetails>("Job Details", new Dictionary<string, object>()
            {
                {"Job", job}
            },
                new DialogOptions(){Width = "900px", Height = "500px"});
        }
    }
}
