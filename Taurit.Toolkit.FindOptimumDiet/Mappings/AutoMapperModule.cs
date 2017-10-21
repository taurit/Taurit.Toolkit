using System;
using AutoMapper;
using Ninject;
using Ninject.Modules;
using Taurit.Toolkit.DietOptimization.Models;
using Taurit.Toolkit.FindOptimumDiet.Models;

namespace Taurit.Toolkit.FindOptimumDiet.Mappings
{
    /// <summary>
    ///     Configuration for AutoMapper. Importantly, contains definition of mappings between various model classes.
    /// </summary>
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
                    .ForCtorParam("fiberTotalDietaryGrams", opt => opt.MapFrom(src => GetValue(src.FiberTotalDietary_Grams)))
                    .ForCtorParam("vitaminAIu", opt => opt.MapFrom(src => GetValue(src.VitaminA_IU)))
                    .ForCtorParam("vitaminCMg", opt => opt.MapFrom(src => GetValue(src.VitaminC_Mg)))
                    .ForCtorParam("ironMg", opt => opt.MapFrom(src => GetValue(src.Iron_Mg)))
                    .ForCtorParam("calciumMg", opt => opt.MapFrom(src => GetValue(src.Calcium_Mg)))
                    .ForCtorParam("magnesiumMg", opt => opt.MapFrom(src => GetValue(src.Magnesium_Mg)))
                    .ForCtorParam("phosphorusMg", opt => opt.MapFrom(src => GetValue(src.Phosphorus_Mg)))
                    .ForCtorParam("potassiumMg", opt => opt.MapFrom(src => GetValue(src.Potassium_Mg)))
                    .ForCtorParam("sodiumMg", opt => opt.MapFrom(src => GetValue(src.Sodium_Mg)))
                    .ForCtorParam("zincMg", opt => opt.MapFrom(src => GetValue(src.Zinc_Mg)))
                    .ForCtorParam("fattyAcidsTotalSaturatedG",
                        opt => opt.MapFrom(src => GetValue(src.FattyAcidsTotalSaturated_Grams)))
                    .ForCtorParam("fattyAcidsTotalMonounsaturatedG",
                        opt => opt.MapFrom(src => GetValue(src.FattyAcidsTotalMonounsaturated_Grams)))
                    .ForCtorParam("fattyAcidsTotalPolyunsaturatedG",
                        opt => opt.MapFrom(src => GetValue(src.FattyAcidsTotalPolyunsaturated_Grams)))
                    .ForCtorParam("fattyAcidsTotalTransG",
                        opt => opt.MapFrom(src => GetValue(src.FattyAcidsTotalTrans_Grams)))
                    .ForCtorParam("cholesterolMg", opt => opt.MapFrom(src => GetValue(src.Cholesterol_Mg)))
                    .ForCtorParam("omega3", opt => opt.MapFrom(src => src.Omega3Total))
                    .ForCtorParam("omega6", opt => opt.MapFrom(src => src.Omega6Total))
                    .ForCtorParam("copperMg", opt => opt.MapFrom(src => GetValue(src.Copper_Mg)))
                    .ForCtorParam("manganeseMg", opt => opt.MapFrom(src => GetValue(src.Manganese_Mg)))
                    .ForCtorParam("seleniumUg", opt => opt.MapFrom(src => GetValue(src.Selenium_Ug)))
                    .ForCtorParam("vitaminB1Mg", opt => opt.MapFrom(src => GetValue(src.Thiamin_Mg)))
                    .ForCtorParam("vitaminB2Mg", opt => opt.MapFrom(src => GetValue(src.Riboflavin_Mg)))
                    .ForCtorParam("vitaminB3Mg", opt => opt.MapFrom(src => GetValue(src.Niacin_Mg)))
                    .ForCtorParam("vitaminB5Mg", opt => opt.MapFrom(src => GetValue(src.PantothenicAcid_Mg)))
                    .ForCtorParam("vitaminB6Mg", opt => opt.MapFrom(src => GetValue(src.VitaminB6A_Mg)))
                    .ForCtorParam("vitaminB12Ug", opt => opt.MapFrom(src => GetValue(src.VitaminB12_Ug)))
                    .ForCtorParam("cholineMg", opt => opt.MapFrom(src => GetValue(src.CholineTotalMg)))
                    .ForCtorParam("vitaminEMg", opt => opt.MapFrom(src => GetValue(src.VitaminE_Mg)))
                    .ForCtorParam("vitaminKUg", opt => opt.MapFrom(src => GetValue(src.VitaminK_Ug)))
                    ;


                // Add all profiles in current assembly
                cfg.AddProfiles(GetType().Assembly);
            });

            return config;
        }

        private String GetValue(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return "0";
            }
            return s;
        }
    }
}