using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AutoMapper;

namespace AbcBooks.Utilities;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<ApplicationUser, ApplicationUserViewModel>();
    }
}
