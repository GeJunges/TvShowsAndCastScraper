using AutoMapper;
using TvMazeScraper.Repository.Entities;
using TvMazeScraper.Service.Models;

namespace TvMazeScraper.Service.Settings;

public class TvShowProfile : Profile
{
    public TvShowProfile()
    {
        CreateMap<Person, PersonResponse>();
        CreateMap<TvShow, TvShowResponse>();
    }
}
