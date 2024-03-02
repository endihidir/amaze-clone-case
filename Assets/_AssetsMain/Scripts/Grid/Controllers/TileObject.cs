using Sirenix.OdinInspector;
using UnityBase.Visitor;
using UnityEngine;

public class TileObject : TileBase, IVisitable, IResettable
{
    [SerializeField] private MeshRenderer _timeMeshRenderer;

    [SerializeField] private Material _defaultMaterial;

    [ReadOnly] [SerializeField] private bool _isPainted;
    public bool IsPainted => _isPainted;

    public void SetMaterial(Material material)
    {
        if(_isPainted) return;

        _isPainted = true;
        
        _timeMeshRenderer.material = material;
    }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public void Reset()
    {
        _isPainted = false;
        
        _timeMeshRenderer.material = _defaultMaterial;
    }

}