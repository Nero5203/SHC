using application.Dto.Auth;
using application.Dto.FileStorage.File;
using application.Dto.FileStorage.Folder;
using application.UseCases.Auth;
using AutoMapper;
using Domain.Entities.FileStorage;

namespace adapters.Driving.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // File mappings
            CreateMap<FileItem, FileDto>().ReverseMap();
            CreateMap<Folder, FolderDto>().ReverseMap(); 

            CreateMap<RegisterDto, RegisterUserRequest>().ReverseMap();
            CreateMap<LoginDto, LoginUserRequest>().ReverseMap();
            CreateMap<LogoutDto, LogoutRequest>().ReverseMap();

            // Add more mappings as needed
        }
    }
}
