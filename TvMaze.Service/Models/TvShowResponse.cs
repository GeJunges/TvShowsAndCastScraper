namespace TvMazeScraper.Service.Models
{
    public class TvShowResponse
    {
        public  int Id  { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PersonResponse> Cast { get; set; } = [];
    }
}
