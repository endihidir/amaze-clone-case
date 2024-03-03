using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBase.ManagerSO
{
    [CreateAssetMenu(menuName = "Game/LevelManagement/LevelData", order = 1)]
    public class LevelSO : SerializedScriptableObject
    {
        #region VARIABLES

        public int index;

        public bool hasUpdateableData;
        
        [Header("Grid Data")] 
        
        [ShowIf("IsMatrixNullOrEmpty")] public int width;
        
        [ShowIf("IsMatrixNullOrEmpty")] public int height;

        [TableMatrix(SquareCells = true, DrawElementMethod = "DrawElement")] public GameObject[,] gridLevel;
        
        #endregion

        #region PROPERTIES
        
        public string Key => name.Replace(" ","");
        
        public bool IsMatrixNullOrEmpty => gridLevel == null || gridLevel.Length < 1;

        public bool IsInitialized
        {
            get => PlayerPrefs.GetInt("Initialized" + Key, 0) == 1;
            set => PlayerPrefs.SetInt("Initialized" + Key, value ? 1 : 0);
        }
        
        #endregion


        [Button, ShowIf("IsMatrixNullOrEmpty")]
        public void CreateEmptyGrid()
        {
            gridLevel = new GameObject[width, height];
        }

        [Button]
        public void SaveToJson()
        {
            if (Application.isPlaying)
            {
                if(IsInitialized) return;
                
                IsInitialized = true;
            }
            
            LevelSOGridDataHelper.SaveToJsonFile(this);
        }
        
#if UNITY_EDITOR
        [Button]
        public void ResetToSavedFile() => LevelSOGridDataHelper.ResetGridToSavedFile(this);

        [Button]
        public void DebugFileData() => LevelSOGridDataHelper.DebugGridFileData(this);

        private GameObject DrawElement(Rect rect, GameObject value, int row, int col)
        {
            return LevelSOGridDrawHelper.DrawGridElement(this, rect, value, row, col);
        }
#endif
    }
}