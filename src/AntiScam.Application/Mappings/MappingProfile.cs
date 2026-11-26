using AutoMapper;
using AntiScam.Application.DTOs.EmergencyContact;
using AntiScam.Application.DTOs.ModelVersion;
using AntiScam.Application.DTOs.PhoneReport;
using AntiScam.Application.DTOs.RelativeContact;
using AntiScam.Application.DTOs.ScamScenario;
using AntiScam.Application.DTOs.TrainingData;
using AntiScam.Domain.Entities;

namespace AntiScam.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ScamScenario, ScamScenarioDto>()
            .ForMember(d => d.ChildScenariosCount, opt => opt.MapFrom(s => s.RelatedScenarios.Count));

        CreateMap<TrainingDataSample, TrainingDataSampleDto>()
            .ForMember(d => d.DataBatch, opt => opt.MapFrom(s => s.BatchId));

        CreateMap<ModelVersion, ModelVersionDto>()
            .ForMember(d => d.VersionLabel, opt => opt.MapFrom(s => s.VersionTag));

        CreateMap<PhoneReport, PhoneReportDto>();

        CreateMap<PhoneReportDetail, PhoneReportDetailDto>()
            .ForMember(d => d.ReporterName, opt => opt.MapFrom(s => s.ReportedBy != null ? s.ReportedBy.FullName : "Unknown"));

        CreateMap<EmergencyContact, EmergencyContactDto>();

        CreateMap<RelativeContact, RelativeContactDto>();
    }
}
