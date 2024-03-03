using System.Collections.Generic;
using System.Linq;
using UnityBase.Manager;
using UnityBase.ManagerSO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class LevelSOGridDataHelper
{
    private const string FOLDER_PATH = "Assets/_AssetsMain/Prefabs/Tile";
    public static void SaveToJsonFile(LevelSO levelSo)
    {
        var jsonDataManager = new JsonDataManager();
        var gridNodeSerializer = new TileSerializer(default);
        levelSo.width = levelSo.gridLevel.GetLength(0);
        levelSo.height = levelSo.gridLevel.GetLength(1);
        var serlializedNodeData = new int[levelSo.width, levelSo.height];

        for (int x = 0; x < levelSo.width; x++)
        {
            for (int z = 0; z < levelSo.height; z++)
            {
                var gridNode = levelSo.gridLevel[x, z]?.GetComponent<TileBase>();
                    
                serlializedNodeData[x, levelSo.height - 1 - z] = gridNodeSerializer.Serialize(gridNode);
            }
        }

        jsonDataManager.Save(levelSo.Key, serlializedNodeData);
    }
    
#if UNITY_EDITOR
    public static void ResetGridToSavedFile(LevelSO levelSO)
    {
        var prefabs = new List<TileBase>();
        var assetPaths = AssetDatabase.FindAssets("t:Prefab", new[] { FOLDER_PATH });

        foreach (var assetPath in assetPaths)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetPath);
            var loadedAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TileBase));

            if (loadedAsset is TileBase gridNode)
                prefabs.Add(gridNode);
                
        }

        var jsonDataManager = new JsonDataManager();
        var gridNodeSerializer = new TileSerializer(default);
        var loadedData = jsonDataManager.Load<int[,]>(levelSO.Key);
            
        if (levelSO.IsMatrixNullOrEmpty)
        {
            levelSO.gridLevel = new GameObject[loadedData.GetLength(0), loadedData.GetLength(1)];
        }

        for (int x = 0; x < loadedData.GetLength(0); x++)
        {
            for (int z = 0; z < loadedData.GetLength(1); z++)
            {
                var objectType = gridNodeSerializer.Deserialize(loadedData[x, z]);

                var selectPrefab = prefabs.FirstOrDefault(x => x.GetType() == objectType);

                if (selectPrefab)
                {
                    levelSO.gridLevel[x, loadedData.GetLength(1) - 1 - z] = selectPrefab.gameObject;
                }
            }
        }
    }

    public static void DebugGridFileData(LevelSO levelSO)
    {
        var jsonDataManager = new JsonDataManager();
        var gridNodeSerializer = new TileSerializer(default);
        var val = jsonDataManager.Load<int[,]>(levelSO.Key);

        for (int i = 0; i < val.GetLength(0); i++)
            for (int j = 0; j < val.GetLength(1); j++)
                Debug.Log(gridNodeSerializer.Deserialize(val[i, j]));
    }
#endif
}

