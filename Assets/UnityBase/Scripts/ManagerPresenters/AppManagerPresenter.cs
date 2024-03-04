using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityBase.Service;
using VContainer.Unity;

namespace UnityBase.Presenter
{
    public class AppManagerPresenter : VContainer.Unity.IInitializable, IPostInitializable, IDisposable
    {
        private readonly IEnumerable<IAppPresenterDataService> _appPresenterDataServices;
        public AppManagerPresenter(IEnumerable<IAppPresenterDataService> appPresenterDataServices) => _appPresenterDataServices = appPresenterDataServices;

        public void Initialize() => _appPresenterDataServices.ForEach(x => x.Initialize());
        public void PostInitialize() => _appPresenterDataServices.ForEach(x => x.Start());
        public void Dispose() => _appPresenterDataServices.ForEach(x => x.Dispose());
    }
}