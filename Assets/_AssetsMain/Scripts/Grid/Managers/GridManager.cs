using Sirenix.Serialization;
using Unity.Mathematics;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;

public class GridManager : IGridDataService, IGridEntity, IGameplayPresenterDataService
{
    private readonly IPoolDataService _poolDataService;
    private readonly ILevelDataService _levelDataService;
    private readonly IGridNodeSerializer _gridNodeSerializer;
    private readonly IJsonDataService _jsonDataService;
    
    private EventBinding<GameStateData> _gameStateBinder = new EventBinding<GameStateData>();
    private Grid<GridNode> _grid;
    private int[,] _gridData;
    private string _levelKey;
    public int Width { get; set; }
    public int Height { get; set; }
    public float NodeSize { get; set; }
    public float3 Padding { get; set; }
    public float3 OriginPos { get; set; }
    public Grid<GridNode> Grid => _grid;

    private LevelObject _currentLevelObject;

    private Tag_GameArena _gameArena;

    public GridManager(ILevelDataService levelDataService, IGridNodeSerializer gridNodeSerializer, IJsonDataService jsonDataService, IPoolDataService poolDataService)
    {
        _levelDataService = levelDataService;
        _gridNodeSerializer = gridNodeSerializer;
        _jsonDataService = jsonDataService;
        _poolDataService = poolDataService;
        _jsonDataService.DataFormat = DataFormat.JSON;
        _gameArena = Object.FindObjectOfType<Tag_GameArena>();
    }

    ~GridManager() => Dispose();
    
    public void Initialize()
    {
        _gameStateBinder.Add(OnStartGameStateTransition);
        EventBus<GameStateData>.AddListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));
        
        LevelManager.OnLevelComplete += OnLevelComplete;
    }

    public void Start() { }
    
    public void Dispose()
    {
        _gameStateBinder.Remove(OnStartGameStateTransition);
        EventBus<GameStateData>.RemoveListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));
        
        LevelManager.OnLevelComplete -= OnLevelComplete;
    }
    
    private void OnStartGameStateTransition(GameStateData gameStateData)
    {
        if (gameStateData.StartState == GameState.GameLoadingState && gameStateData.EndState == GameState.GamePlayState)
        {
            UpdateGridData();
            GenerateGrid(false);
        }
    }

    private void OnLevelComplete()
    {
        UpdateGridData();
        GenerateGrid(true);
    }
    
    private void UpdateGridData()
    {
        _levelKey = _levelDataService.GetCurrentLevelData().Key;
        _gridData = _jsonDataService.Load<int[,]>(_levelKey);
        Width = _gridData.GetLength(0);
        Height = _gridData.GetLength(1);
        NodeSize = 1f;
        Padding = 0f;
        OriginPos = new float3(GetOriginXPos(), 0f, GetOriginZPos());
        _grid = new Grid<GridNode>(this);
    }

    private void GenerateGrid(bool onLevelComplete)
    {
        _currentLevelObject = _poolDataService.GetObject<LevelObject>(onLevelComplete ? 0.75f : 0f, 0f);
        
        _currentLevelObject.transform.SetParent(_gameArena.transform);
        
        _currentLevelObject.name = _levelKey;
        
        _currentLevelObject.transform.position = onLevelComplete ? new Vector3(_grid.Width * (NodeSize + Padding.x), 0f, 0f) : Vector3.zero;

        for (int x = 0; x < Height; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                GridNode gridNode = _gridNodeSerializer.Deserialize<GridNode>(_gridData[x, z]);
                gridNode.index = _grid.CalculateIndex(x, z);
                var gridTransform = gridNode.transform;
                gridTransform.SetParent(_currentLevelObject.GridsParent.transform);
                gridTransform.localPosition = _grid.GetWorldPosition(x, 0, z);
                gridTransform.localScale = Vector3.one * NodeSize;
                _grid.SetGridObject(x, z, gridNode);
            }
        }
    }

    private float GetOriginXPos()
    {
        var offsetMultiplier = (NodeSize + Padding.x);
        var xPos = ((offsetMultiplier - (Width * offsetMultiplier)) * 0.5f);
        return xPos;
    }

    private float GetOriginZPos()
    {
        var offsetMultiplier = (NodeSize + Padding.z);
        var zPos = ((offsetMultiplier - (Height * offsetMultiplier)) * 0.5f);
        return zPos;
    }
}