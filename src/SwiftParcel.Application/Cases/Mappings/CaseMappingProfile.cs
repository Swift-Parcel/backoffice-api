using AutoMapper;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Cases.Mappings;

public class CaseMappingProfile : Profile
{
    public CaseMappingProfile()
    {
        CreateMap<Case, CaseSummaryDto>()
            .ForCtorParam(nameof(CaseSummaryDto.NoteCount), 
                opt => opt.MapFrom(src => src.Notes.Count))
            
            .ForCtorParam(nameof(CaseSummaryDto.ParcelCount), 
                opt => opt.MapFrom(src => src.Parcels.Count));
    }
}