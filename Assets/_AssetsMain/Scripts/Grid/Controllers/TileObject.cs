using UnityEngine;

public class TileObject : TileBase, IResettable
{
    [SerializeField] private MeshRenderer _timeMeshRenderer;

    [SerializeField] private Material _defaultMaterial;
    

    private bool _isPainted;
    public bool IsPainted => _isPainted;

    public void SetMaterial(Material material)
    {
        if(_isPainted) return;

        _isPainted = true;
        
        _timeMeshRenderer.material = material;
    }

    public void Reset()
    {
        _isPainted = false;
        
        _timeMeshRenderer.material = _defaultMaterial;
    }
}