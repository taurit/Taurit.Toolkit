using Ninject.Modules;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.FindOptimumDiet.Mappings
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<DietCharacteristicsCalculator>().To<DietCharacteristicsCalculator>();
            Bind<DietCharacteristicsDistanceCalculator>().To<DietCharacteristicsDistanceCalculator>();
            Bind<DietPlanPresenter>().To<DietPlanPresenter>();
            Bind<ProductLoader>().To<ProductLoader>();
            Bind<FindOptimumDiet>().To<FindOptimumDiet>();
        }
    }
}