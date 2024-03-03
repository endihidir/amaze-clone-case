using System;
using Unity.Mathematics;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.ManagerSO;
using UnityBase.Service;
using UnityEngine;

public class GridManager : IGridDataService, IGridEntity, IGameplayPresenterDataService
{
    public static Action OnAllTilesPainted;
    public static Action<CoinTileObject> OnCollectCoinTile;
    private EventBinding<GameStateData> _gameStateBinder = new EventBinding<GameStateData>();
   
    private readonly IPoolDataService _poolDataService;
    private readonly ILevelDataService _levelDataService;
    private readonly ITileSerializer _tileSerializer;
    private readonly IJsonDataService _jsonDataService;
    private readonly IGameplayDataService _gameplayDataService;
    
    private LevelSO _levelData;
    private Grid<TileBase> _gridData;
    private int[,] _serializedGridData;
    private string _levelKey;
    
    private LevelObject _currentLevelObject;
    
    private Tag_GameArena _gameArena;
    public int Width { get; set; }
    public int Height { get; set; }
    public float NodeSize { get; set; }
    public float3 Padding { get; set; }
    public float3 OriginPos { get; set; }
    public Grid<TileBase> Grid => _gridData;

    public GridManager(ILevelDataService levelDataService, ITileSerializer tileSerializer, IJsonDataService jsonDataService, IPoolDataService poolDataService, IGameplayDataService gameplayDataService)
    {
        _levelDataService = levelDataService;
        _tileSerializer = tileSerializer;
        _jsonDataService = jsonDataService;
        _poolDataService = poolDataService;
        _gameplayDataService = gameplayDataService;
        _gameArena = UnityEngine.Object.FindObjectOfType<Tag_GameArena>();
    }

    ~GridManager() => Dispose();
    
    public void Initialize()
    {
        _gameStateBinder.Add(OnStartGameStateTransition);
        EventBus<GameStateData>.AddListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));

        OnAllTilesPainted += AllTilesPainted;
        OnCollectCoinTile += CollectCoinTile;
    }

    public void Start() { }
    
    public void Dispose()
    {
        _gameStateBinder.Remove(OnStartGameStateTransition);
        EventBus<GameStateData>.RemoveListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));
        
        OnAllTilesPainted -= AllTilesPainted;
        OnCollectCoinTile -= CollectCoinTile;
    }

    private void OnStartGameStateTransition(GameStateData gameStateData)
    {
        var isGameplayStarted = gameStateData.StartState == GameState.GameLoadingState && gameStateData.EndState == GameState.GamePlayState;
        var isPassedToNextLevel = gameStateData.StartState == GameState.GameSuccessState && gameStateData.EndState == GameState.GamePlayState;
        
        if (isGameplayStarted)
        {
            UpdateGridData();
            GenerateGrid(false);
        }
        else if (isPassedToNextLevel)
        {
            HideCurrentLevel();
            UpdateGridData();
            GenerateGrid(true);
        }
    }

    private void HideCurrentLevel()
    {
        _levelData.ResetUpdatedLevelData();
        var endXPos = _gridData.Width * (NodeSize + Padding.x) * -5;
        _currentLevelObject.SetEndXPos(endXPos);
        _poolDataService.HideObject(_currentLevelObject, 0.75f, 0f);
    }

    private void UpdateGridData()
    {
        _levelData = _levelDataService.GetCurrentLevelData();
        _levelData.SaveToJson();
        _levelKey = _levelData.Key;
        _serializedGridData = _jsonDataService.Load<int[,]>(_levelKey);
        Width = _serializedGridData.GetLength(0);
        Height = _serializedGridData.GetLength(1);
        NodeSize = 1f;
        Padding = 0f;
        OriginPos = new float3(GetGridOriginXPos(), 0f, GetGridOriginZPos());
        _gridData = new Grid<TileBase>(this);
    }

    private void GenerateGrid(bool onLevelComplete)
    {
        _currentLevelObject = _poolDataService.GetObject<LevelObject>(onLevelComplete ? 0.75f : 0f, 0f);
        _currentLevelObject.transform.SetParent(_gameArena.transform);
        _currentLevelObject.name = _levelKey;
        _currentLevelObject.transform.position = onLevelComplete ? new Vector3(_gridData.Width * (NodeSize + Padding.x) * 5f, 0f, 0f) : Vector3.zero;

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                TileBase tileBase = _tileSerializer.Deserialize<TileBase>(_serializedGridData[x, z]);
                tileBase.index = _gridData.CalculateIndex(x, z);
                var tileTransform = tileBase.transform;
                tileTransform.SetParent(_currentLevelObject.GridsParent.transform);
                tileTransform.localPosition = _gridData.GetWorldPosition(x, 0, z);
                tileTransform.localScale = Vector3.one * NodeSize;
                _gridData.SetGridObject(x, z, tileBase);
                ArrangeTile(tileBase);
            }
        }
    }
    
    private void AllTilesPainted()
    {
        _gameplayDataService.ChangeGameState(GameState.GameSuccessState, 0f);
    }
    
    private void CollectCoinTile(CoinTileObject coinTileObject)
    {
        for (int x = 0; x < _gridData.Width; x++)
        {
            for (int z = 0; z < _gridData.Height; z++)
            {
                var tileObject = _gridData.GetGridObject(x, z);
                
                if (tileObject is not CoinTileObject coinTile) continue;
                
                if (coinTile == coinTileObject)
                    _serializedGridData[x, z] = _tileSerializer.SerializeOnCoinCollect(coinTileObject);
            }
        }
        
        _jsonDataService.Save(_levelKey, _serializedGridData);
    }

    private void ArrangeTile(TileBase tileBase)
    {
        if (tileBase is PlayerTileObject playerTileObject)
        {
            var ballController = _poolDataService.GetObject<BallController>(0f, 0f);
            ballController.transform.position = playerTileObject.transform.position;
            ballController.transform.parent = _currentLevelObject.BallsParent;
            ballController.PathProvider.SetTileIndex(playerTileObject.index);
            ballController.PathProvider.SetGridData(_gridData);
            playerTileObject.SetMaterial(ballController.MaterialProvider.CurrentStampMaterial);
        }
    }

    private float GetGridOriginXPos()
    {
        var offsetMultiplier = (NodeSize + Padding.x);
        var xPos = ((offsetMultiplier - (Width * offsetMultiplier)) * 0.5f);
        return xPos;
    }

    private float GetGridOriginZPos()
    {
        var offsetMultiplier = (NodeSize + Padding.z);
        var zPos = ((offsetMultiplier - (Height * offsetMultiplier)) * 0.5f);
        return zPos;
    }
}