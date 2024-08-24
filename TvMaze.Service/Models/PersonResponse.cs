namespace TvMazeScraper.Service.Models
{
    public class PersonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? Birthday { get; set; }
    }
}
