using api.Dto.FileStorage.File;
using api.Dto.FileStorage.Folder;
using AutoMapper;
using Domain.Entities.FileStorage;

namespace application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // File mappings
            CreateMap<FileItem, FileDto>().ReverseMap();
            CreateMap<Folder, FolderDto>().ReverseMap(); 

            // Add more mappings as needed
        }
    }
}
