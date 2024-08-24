namespace TvMazeScraper.Repository.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? Birthday { get; set; }

        public ICollection<TvShow> TvShows { get; set; } = [];
    }
}