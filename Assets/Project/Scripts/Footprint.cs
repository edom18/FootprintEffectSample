using UnityEngine;

public class Footprint : MonoBehaviour
{
    [SerializeField] private ComputeShader _computeShader = null;

    private int _kernelID = 0;
    private bool _hasInitialized = false;
    private RenderTexture _target = null;

    private ComputeShader _runtimeShader = null;
    private ComputeShader RuntimeShader => _runtimeShader ?? (_runtimeShader = Instantiate(_computeShader));
    
    private void Update()
    {
        if (!_hasInitialized) return;
        
        RuntimeShader.Dispatch(_kernelID, _target.width / 8, _target.height / 8, 1);
    }

    public void SetTexture(RenderTexture target)
    {
        _kernelID = RuntimeShader.FindKernel("Update");
        _target = target;
        RuntimeShader.SetTexture(_kernelID, "_Footprint", target);
        _hasInitialized = true;
    }
}
