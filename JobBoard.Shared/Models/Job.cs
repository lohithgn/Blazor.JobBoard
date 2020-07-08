namespace JobBoard.Shared.Models
{
    public class Job : CosmosEntity
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string SubtitleUrl { get; set; }
        public string CallToActionUrl { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
    }
}
