using Refit;

namespace JobBoard.Client.Models
{
    public class JobSearchParams
    {
        [AliasAs("q")]
        public string SearchString { get; set; }
        [AliasAs("locations")]
        public string Locations { get; set; }
        [AliasAs("types")]
        public string Types { get; set; }
        [AliasAs("page")]
        public string Page { get; set; }
    }
}
