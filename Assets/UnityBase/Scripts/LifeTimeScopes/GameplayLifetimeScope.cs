using UnityBase.Manager;
using UnityBase.Presenter;
using VContainer;
using VContainer.Unity;

namespace UnityBase.BaseLifetimeScope
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameplayManagerPresenter>();

            builder.RegisterComponentInHierarchy<CoinUI>().AsImplementedInterfaces();
            
            builder.Register<GameplayManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CinemachineManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GridManager>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<TileSerializer>(Lifetime.Scoped).AsImplementedInterfaces();
        }
    }
}
