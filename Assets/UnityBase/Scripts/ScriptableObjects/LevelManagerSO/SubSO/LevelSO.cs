using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityBase.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityBase.ManagerSO
{
    [CreateAssetMenu(menuName = "Game/LevelManagement/LevelData", order = 1)]
    public class LevelSO : SerializedScriptableObject
    {
        private const string FOLDER_PATH = "Assets/_AssetsMain/Prefabs/Grid";
        
        public int index;
        public string Key => name;

        [Header("Grid Data")] [ShowIf("IsMatrixNullOrEmpty")]
        public int width;
        [ShowIf("IsMatrixNullOrEmpty")] 
        public int height;

        [TableMatrix(SquareCells = true, DrawElementMethod = "DrawElement")]
        public GameObject[,] gridLevel;

        private bool IsMatrixNullOrEmpty => gridLevel == null || gridLevel.Length < 1;
      

        [Button, ShowIf("IsMatrixNullOrEmpty")]
        public void CreateGrid()
        {
            gridLevel = new GameObject[width, height];
        }

        [Button]
        public void SaveGridToJsonFile()
        {
            var jsonDataManager = new JsonDataManager{ DataFormat = DataFormat.JSON };
            var gridNodeSerializer = new GridNodeSerializer(default);
            var serlializedNodeData = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var gridNode = gridLevel[x, z]?.GetComponent<TileBase>();
                    
                    serlializedNodeData[x, height - 1 - z] = gridNodeSerializer.Serialize(gridNode);
                }
            }

            jsonDataManager.Save(Key, serlializedNodeData);
        }

#if UNITY_EDITOR
        [Button]
        public void ResetToSavedFile()
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

            var jsonDataManager = new JsonDataManager() { DataFormat = DataFormat.JSON };
            var gridNodeSerializer = new GridNodeSerializer(default);
            var loadedData = jsonDataManager.Load<int[,]>(Key);
            
            if (IsMatrixNullOrEmpty)
            {
                gridLevel = new GameObject[loadedData.GetLength(0), loadedData.GetLength(1)];
            }

            for (int i = 0; i < loadedData.GetLength(0); i++)
            {
                for (int j = 0; j < loadedData.GetLength(1); j++)
                {
                    var objectType = gridNodeSerializer.Deserialize(loadedData[i, j]);

                    var selectPrefab = prefabs.FirstOrDefault(x => x.GetType() == objectType);

                    if (selectPrefab)
                    {
                        gridLevel[i, j] = selectPrefab.gameObject;
                    }
                }
            }

        }
#endif

        [Button]
        public void DebugFileData()
        {
            var jsonDataManager = new JsonDataManager() { DataFormat = DataFormat.JSON };
            var gridNodeSerializer = new GridNodeSerializer(default);
            var val = jsonDataManager.Load<int[,]>(Key);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    Debug.Log(gridNodeSerializer.Deserialize(val[i, j]));
        }

        private GameObject DrawElement(Rect rect, GameObject value, int row, int col)
        {
            var gridNode = gridLevel[row, col];

            if (gridNode != null && gridNode.TryGetComponent<TileBase>(out var component))
            {
                var content = (component.icon != null) ? new GUIContent(component.icon) : GUIContent.none;

                gridNode = (GameObject)SirenixEditorFields.UnityPreviewObjectField(rect, content, value, typeof(GameObject), false);

#if UNITY_EDITOR
                if (component.icon != null)
                {
                    EditorGUI.DrawPreviewTexture(rect, value.GetComponent<TileBase>().icon);
                }
#endif
            }
            else
            {
                gridNode = (GameObject)SirenixEditorFields.UnityPreviewObjectField(rect, GUIContent.none, value, typeof(GameObject), false);
            }

            return gridNode;
        }
    }
}