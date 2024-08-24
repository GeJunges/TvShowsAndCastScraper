namespace TvMazeScraper.Repository.Entities
{
    public class TvShow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Person> Cast { get; set; } = [];
    }
}
