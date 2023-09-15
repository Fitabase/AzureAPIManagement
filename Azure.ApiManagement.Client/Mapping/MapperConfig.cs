using AutoMapper;
using Azure.ResourceManager.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;

namespace Azure.ApiManagement.Client.MappingProfiles
{
    public class MapperConfig
    {
        internal static Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserContractData>().ReverseMap();
                cfg.CreateMap<Product, ApiManagementProductData>().ReverseMap();
                cfg.CreateMap<Group, ApiManagementGroupData>().ReverseMap();
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
