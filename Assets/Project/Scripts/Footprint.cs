using UnityEngine;

public class Footprint : MonoBehaviour
{
    [SerializeField] private ComputeShader _computeShader = null;

    private int _kernelID = 0;
    private bool _hasInitialized = false;
    private RenderTexture _target = null;
    private RenderTexture _baseTarget = null;

    private ComputeShader _runtimeShader = null;

    private ComputeShader RuntimeShader
    {
        get
        {
            if (_runtimeShader == null)
            {
                _runtimeShader = Instantiate(_computeShader);
                _kernelID = _runtimeShader.FindKernel("Update");
            }

            return _runtimeShader;
        }
    }

    private void Update()
    {
        if (!_hasInitialized) return;

        RuntimeShader.Dispatch(_kernelID, _target.width / 8, _target.height / 8, 1);
    }

    public void SetBaseTexture(Texture baseTex)
    {
        RuntimeShader.SetTexture(_kernelID, "_BaseTex", baseTex);
    }

    public void SetTexture(RenderTexture target)
    {
        _target = target;
        RuntimeShader.SetTexture(_kernelID, "_Footprint", target);
        _hasInitialized = true;
    }
}