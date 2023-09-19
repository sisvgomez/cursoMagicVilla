using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig :Profile
    {

        // creamos el contructor
        public MappingConfig() {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto,Villa>();

            CreateMap<Villa,VillaCreateDto>().ReverseMap(); // esto lo mismo de arriba en una solo linea
            CreateMap<Villa, VillaUpdateDto>().ReverseMap(); 
        }
    }
}
