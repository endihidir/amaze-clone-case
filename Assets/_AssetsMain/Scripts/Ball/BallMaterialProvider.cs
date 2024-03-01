using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallMaterialProvider : MonoBehaviour
{
    [SerializeField] private MeshRenderer _ballMeshRenderer;

    [SerializeField] private Material[] _ballStampMaterials;
    
    [ReadOnly] [SerializeField] private Material _currentStampMaterial;
    public Material CurrentStampMaterial => _currentStampMaterial;
    private void Awake() => SetRandomMaterial();

    private void SetRandomMaterial()
    {
        var index = Random.Range(0, _ballStampMaterials.Length);
        _currentStampMaterial = _ballStampMaterials[index];
    }

}