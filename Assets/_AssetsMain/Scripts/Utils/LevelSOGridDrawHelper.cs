#if UNITY_EDITOR

using Sirenix.Utilities.Editor;
using UnityBase.ManagerSO;
using UnityEditor;
using UnityEngine;

public static class LevelSOGridDrawHelper
{
    public static GameObject DrawGridElement(LevelSO levelSO, Rect rect, GameObject value, int row, int col)
    {
        var gridNode = levelSO.gridLevel[row, col];

        if (gridNode != null && gridNode.TryGetComponent<TileBase>(out var component))
        {
            var content = (component.icon != null) ? new GUIContent(component.icon) : GUIContent.none;

            gridNode = (GameObject)SirenixEditorFields.UnityPreviewObjectField(rect, content, value, typeof(GameObject), false);
                
            if (component.icon != null)
            {
                EditorGUI.DrawPreviewTexture(rect, value.GetComponent<TileBase>().icon);
            }
        }
        else
        {
            gridNode = (GameObject)SirenixEditorFields.UnityPreviewObjectField(rect, GUIContent.none, value, typeof(GameObject), false);
        }

        return gridNode;
    }
}

#endif