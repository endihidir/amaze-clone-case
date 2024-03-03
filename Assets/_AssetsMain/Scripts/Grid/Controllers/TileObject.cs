using DG.Tweening;
using Sirenix.OdinInspector;
using UnityBase.Visitor;
using UnityEngine;

public class TileObject : TileBase, IVisitable, IResettable
{
    [SerializeField] private Transform _meshHandler;
    
    [SerializeField] private MeshRenderer _timeMeshRenderer;

    [SerializeField] private Material _defaultMaterial, _borderMaterial;

    [ReadOnly] [SerializeField] private bool _isPainted;
    public bool IsPainted => _isPainted;

    private Tween _tileAnim;

    public void SetMaterial(Material material)
    {
        if(_isPainted) return;

        _isPainted = true;

        var materials = _timeMeshRenderer.materials;
        materials[0] = _borderMaterial;
        materials[1] = material;
        _timeMeshRenderer.materials = materials;
    }

    public void PlayTileAnim(Direction direction)
    {
        var dir = direction is Direction.Up or Direction.Down ? Vector3.right : direction is Direction.Right ? Vector3.back : Vector3.forward;
        
        _tileAnim?.Kill(true);
        _tileAnim = _meshHandler.DOLocalRotate(dir * 360f, 0.3f, RotateMode.LocalAxisAdd)
                                .SetEase(Ease.InOutQuad)
                                .OnComplete(()=> _meshHandler.rotation = Quaternion.identity);
    }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public virtual void Reset()
    {
        _isPainted = false;

        var materials = _timeMeshRenderer.materials;
        
        for (var i = 0; i < _timeMeshRenderer.materials.Length; i++)
        {
            materials[i] = _defaultMaterial;
        }
        
        _timeMeshRenderer.materials = materials;
        
        _meshHandler.rotation = Quaternion.identity;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tileAnim?.Kill();
    }
}