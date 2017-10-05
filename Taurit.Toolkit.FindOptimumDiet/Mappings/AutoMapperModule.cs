using AutoMapper;
using Ninject;
using Ninject.Modules;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.FindOptimumDiet.Models;

namespace Taurit.Toolkit.FindOptimumDiet.Mappings
{
    public sealed class AutoMapperModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<IValueResolver<UsdaProduct, FoodProduct, bool>>().To<MyResolver>();

            var mapperConfiguration = CreateConfiguration();
            Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();

            // This teaches Ninject how to create automapper instances say if for instance
            // MyResolver has a constructor with a parameter that needs to be injected
            Bind<IMapper>().ToMethod(ctx =>
                new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
        }

        private MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsdaProduct, FoodProduct>()
                    .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
                    .ForCtorParam("energyKcal", opt => opt.MapFrom(src => src.Energy_Kcal))
                    .ForCtorParam("percentProtein", opt => opt.MapFrom(src => src.Protein_Grams))
                    .ForCtorParam("percentFat", opt => opt.MapFrom(src => src.Fat_Grams))
                    .ForCtorParam("percentCarb", opt => opt.MapFrom(src => src.Carbohydrate_Grams))
                    
                    ;


                // Add all profiles in current assembly
                cfg.AddProfiles(GetType().Assembly);
            });

            return config;
        }
    }
}