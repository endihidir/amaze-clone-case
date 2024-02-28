using UnityBase.Service;

public class GridManager : IGridDataService, IGameplayPresenterDataService
{
    private readonly ILevelDataService _levelDataService;
    
    public GridManager(ILevelDataService levelDataService)
    {
        _levelDataService = levelDataService;
    }

    ~GridManager()
    {
        
    }
    
    public void Initialize()
    {
        
    }

    public void Start()
    {
        
    }

    public void Dispose()
    {
        
    }
}