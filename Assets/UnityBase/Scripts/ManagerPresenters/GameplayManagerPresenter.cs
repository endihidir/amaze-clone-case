using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityBase.Manager;
using UnityBase.Service;
using VContainer;
using VContainer.Unity;

namespace UnityBase.Presenter
{
    public class GameplayManagerPresenter : IInitializable, IPostInitializable, IDisposable
    {
        private readonly IEnumerable<IGameplayPresenterDataService> _gameplayPresenterDataServices;
        
        public GameplayManagerPresenter(IObjectResolver objectResolver, IEnumerable<IGameplayPresenterDataService> gameplayPresenterDataServices)
        {
            var poolManager = objectResolver.Resolve<IPoolDataService>() as PoolManager;
            
            poolManager?.UpdateAllResolvers(objectResolver);

            _gameplayPresenterDataServices = gameplayPresenterDataServices;
        }
        
        public void Initialize() => _gameplayPresenterDataServices.ForEach(x => x.Initialize());
        public void PostInitialize() => _gameplayPresenterDataServices.ForEach(x => x.Start());
        public void Dispose() => _gameplayPresenterDataServices.ForEach(x => x.Dispose());
    }
}