using Ninject.Modules;
using Taurit.Toolkit.DietOptimization.Services;

namespace Taurit.Toolkit.FindOptimumDiet.Mappings
{
    /// <summary>
    ///     Ninject dependency injection configuration.
    ///     Classes that we want resolved by Ninject must appear on this list.
    /// </summary>
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<DietCharacteristicsCalculator>().To<DietCharacteristicsCalculator>();
            Bind<ScoreCalculator>().To<ScoreCalculator>();
            Bind<DietPlanPresenter>().To<DietPlanPresenter>();
            Bind<ProductLoader>().To<ProductLoader>();
            Bind<FindOptimumDiet>().To<FindOptimumDiet>();
        }
    }
}