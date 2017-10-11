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

            MapperConfiguration mapperConfiguration = CreateConfiguration();
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
                    .ForCtorParam("fiberTotalDietaryGrams", opt => opt.MapFrom(src => src.FiberTotalDietary_Grams))
                    .ForCtorParam("vitaminAIu", opt => opt.MapFrom(src => src.VitaminA_IU))
                    .ForCtorParam("vitaminCMg", opt => opt.MapFrom(src => src.VitaminC_Mg))
                    .ForCtorParam("ironMg", opt => opt.MapFrom(src => src.Iron_Mg))
                    .ForCtorParam("calciumMg", opt => opt.MapFrom(src => src.Calcium_Mg))
                    .ForCtorParam("magnesiumMg", opt => opt.MapFrom(src => src.Magnesium_Mg))
                    .ForCtorParam("phosphorusMg", opt => opt.MapFrom(src => src.Phosphorus_Mg))
                    .ForCtorParam("potassiumMg", opt => opt.MapFrom(src => src.Potassium_Mg))
                    .ForCtorParam("sodiumMg", opt => opt.MapFrom(src => src.Sodium_Mg))
                    .ForCtorParam("zincMg", opt => opt.MapFrom(src => src.Zinc_Mg))
                    ;


                // Add all profiles in current assembly
                cfg.AddProfiles(GetType().Assembly);
            });

            return config;
        }
    }
}