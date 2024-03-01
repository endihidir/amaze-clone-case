using System;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridManager : IGridDataService, IGridEntity, IGameplayPresenterDataService
{
    private readonly IPoolDataService _poolDataService;
    private readonly ILevelDataService _levelDataService;
    private readonly IGridNodeSerializer _gridNodeSerializer;
    private readonly IJsonDataService _jsonDataService;
    private readonly IGameplayDataService _gameplayDataService;

    public static Action OnAllTilesPainted;
    
    private EventBinding<GameStateData> _gameStateBinder = new EventBinding<GameStateData>();
    private Grid<TileBase> _gridData;
    private int[,] _serializedGridData;
    private string _levelKey;
    public int Width { get; set; }
    public int Height { get; set; }
    public float NodeSize { get; set; }
    public float3 Padding { get; set; }
    public float3 OriginPos { get; set; }
    public Grid<TileBase> Grid => _gridData;

    private LevelObject _currentLevelObject;

    private Tag_GameArena _gameArena;

    public GridManager(ILevelDataService levelDataService, IGridNodeSerializer gridNodeSerializer, IJsonDataService jsonDataService, 
        IPoolDataService poolDataService, IGameplayDataService gameplayDataService)
    {
        _levelDataService = levelDataService;
        _gridNodeSerializer = gridNodeSerializer;
        _jsonDataService = jsonDataService;
        _poolDataService = poolDataService;
        _gameplayDataService = gameplayDataService;
        _jsonDataService.DataFormat = DataFormat.JSON;
        _gameArena = UnityEngine.Object.FindObjectOfType<Tag_GameArena>();
    }

    ~GridManager() => Dispose();
    
    public void Initialize()
    {
        _gameStateBinder.Add(OnStartGameStateTransition);
        EventBus<GameStateData>.AddListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));

        OnAllTilesPainted += AllTilesPainted;
    }

    public void Start() { }
    
    public void Dispose()
    {
        _gameStateBinder.Remove(OnStartGameStateTransition);
        EventBus<GameStateData>.RemoveListener(_gameStateBinder, GameStateData.GetChannel(TransitionState.Start));
        
        OnAllTilesPainted += AllTilesPainted;
    }

    private void AllTilesPainted()
    {
        _currentLevelObject.DeactivateInput();
        
        _gameplayDataService.ChangeGameState(GameState.GameSuccessState, 0f);
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
        var endXPos = _gridData.Width * (NodeSize + Padding.x) * -5;
        _currentLevelObject.SetEndPos(endXPos);
        _poolDataService.HideObject(_currentLevelObject, 0.75f, 0f);
    }

    private void UpdateGridData()
    {
        _levelKey = _levelDataService.GetCurrentLevelData().Key;
        _serializedGridData = _jsonDataService.Load<int[,]>(_levelKey);
        Width = _serializedGridData.GetLength(0);
        Height = _serializedGridData.GetLength(1);
        NodeSize = 1f;
        Padding = 0f;
        OriginPos = new float3(GetOriginXPos(), 0f, GetOriginZPos());
        _gridData = new Grid<TileBase>(this);
    }

    private void GenerateGrid(bool onLevelComplete)
    {
        _currentLevelObject = _poolDataService.GetObject<LevelObject>(onLevelComplete ? 0.75f : 0f, 0f);
        _currentLevelObject.transform.SetParent(_gameArena.transform);
        _currentLevelObject.name = _levelKey;
        _currentLevelObject.transform.position = onLevelComplete ? new Vector3(_gridData.Width * (NodeSize + Padding.x) * 5f, 0f, 0f) : Vector3.zero;

        for (int x = 0; x < Height; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                TileBase tileBase = _gridNodeSerializer.Deserialize<TileBase>(_serializedGridData[x, z]);
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

    private void ArrangeTile(TileBase tileBase)
    {
        if (tileBase is PlayerTileObject playerTileObject)
        {
            var ballController = _poolDataService.GetObject<BallController>(0f, 0f);
            ballController.transform.parent = _currentLevelObject.BallsParent;
            ballController.transform.position = playerTileObject.transform.position;
            ballController.PathProvider.SetTileIndex(playerTileObject.index);
            ballController.PathProvider.SetGridData(_gridData);
            playerTileObject.SetMaterial(ballController.MaterialProvider.CurrentStampMaterial);
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