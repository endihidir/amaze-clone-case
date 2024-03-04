using System.Collections.Generic;
using System.Linq;
using UnityBase.EventBus;
using UnityBase.Manager.Data;
using UnityEngine;

public class CoinDrawer : MonoBehaviour
{
    [SerializeField] private Mesh _collectibleMesh;
    
    [SerializeField] private Material _collectibleMaterial;
    
    private List<ICoinDrawer> _collectibles;

    private List<Matrix4x4> _matrices = new List<Matrix4x4>();

    private EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>();

    private bool _isInitialized;

    public void Initialize()
    {
        _collectibles = FindObjectsOfType<MonoBehaviour>().OfType<ICoinDrawer>().ToList();

        for (int i = 0; i < _collectibles.Count; i++)
        {
            var collectibleT = _collectibles[i].Transform;
            
            _matrices.Add(Matrix4x4.TRS(collectibleT.position, collectibleT.rotation, collectibleT.localScale));
        }

        _isInitialized = _collectibles.Count > 1;
        
        enabled = _isInitialized;
    }

    private void Update()
    {
        if(!_isInitialized) return;
        
        for (int i = 0; i < _collectibles.Count; i++)
        {
            var collectibleT = _collectibles[i].Transform;
            
            _matrices[i] = Matrix4x4.TRS(collectibleT.position, collectibleT.rotation, collectibleT.localScale);
            
            if (_collectibles[i].IsCoinDisabled)
            {
                _collectibles.RemoveAt(i);
                _matrices.RemoveAt(i);
            }
        }
        
        Graphics.DrawMeshInstanced(_collectibleMesh, 0, _collectibleMaterial, _matrices);
    }

    public void Disable()
    {
        _isInitialized = false;
        enabled = false;
    }
}