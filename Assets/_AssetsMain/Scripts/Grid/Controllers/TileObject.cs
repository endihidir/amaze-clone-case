using Sirenix.OdinInspector;
using UnityBase.Visitor;
using UnityEngine;

public class TileObject : TileBase, IVisitable, IResettable
{
    [SerializeField] private MeshRenderer _timeMeshRenderer;

    [SerializeField] private Material _defaultMaterial, _borderMaterial;

    [ReadOnly] [SerializeField] private bool _isPainted;
    public bool IsPainted => _isPainted;

    public void SetMaterial(Material material)
    {
        if(_isPainted) return;

        _isPainted = true;

        var materials = _timeMeshRenderer.materials;
        materials[0] = _borderMaterial;
        materials[1] = material;
        _timeMeshRenderer.materials = materials;
    }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public void Reset()
    {
        _isPainted = false;

        var materials = _timeMeshRenderer.materials;
        
        for (var i = 0; i < _timeMeshRenderer.materials.Length; i++)
        {
            materials[i] = _defaultMaterial;
        }
        
        _timeMeshRenderer.materials = materials;
    }

}