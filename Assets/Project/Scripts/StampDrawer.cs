using UnityEngine;

public class StampDrawer : MonoBehaviour
{
    private class SwapBuffer
    {
        private RenderTexture[] _buffers = new RenderTexture[2];

        public RenderTexture Current
        {
            get { return _buffers[0]; }
        }

        public RenderTexture Next
        {
            get { return _buffers[1]; }
        }

        private Color _initColor = Color.clear;

        /// <summary>
        /// Swap buffer constructor.
        /// </summary>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <param name="initColor">Initialize color (Clear color)</param>
        public SwapBuffer(int width, int height, Color initColor)
        {
            _initColor = initColor;

            for (int i = 0; i < _buffers.Length; i++)
            {
                _buffers[i] = CreateRenderTexture(width, height);
            }
        }

        private void ClearTexture(RenderTexture texture)
        {
            RenderTexture temp = RenderTexture.active;

            RenderTexture.active = texture;
            GL.Clear(true, true, _initColor);

            RenderTexture.active = temp;
        }

        private RenderTexture CreateRenderTexture(int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
            rt.enableRandomWrite = true;
            rt.Create();
            ClearTexture(rt);

            return rt;
        }

        /// <summary>
        /// Release the all render textures.
        /// </summary>
        public void Release()
        {
            foreach (var buf in _buffers)
            {
                buf.Release();
            }
        }

        /// <summary>
        /// Swap render textures.
        /// </summary>
        public void Swap()
        {
            RenderTexture tmp = _buffers[0];
            _buffers[0] = _buffers[1];
            _buffers[1] = tmp;
        }
    }

    [SerializeField] private Color _initColor = Color.clear;
    [SerializeField] private GameObject _target = null;
    [SerializeField] private Footprint _footprint = null;

    private int _defaultStampTextureSize = 1024;
    private SwapBuffer _swapBuffer = null;
    private int _mainTexId = 0;

    private Mesh _mesh = null;
    private Renderer _targetRenderer = null;

    private void SetTexture(RenderTexture target)
    {
        _footprint.SetTexture(target);
        _targetRenderer.sharedMaterials[0].mainTexture = target;
    }

    public void Setup()
    {
        _mainTexId = Shader.PropertyToID("_MainTex");

        _targetRenderer = _target.GetComponent<Renderer>();
        Texture texture = _targetRenderer.sharedMaterials[0].mainTexture;

        _footprint.SetBaseTexture(texture);

        int width = 0;
        int height = 0;

        if (texture == null)
        {
            width = _defaultStampTextureSize;
            height = _defaultStampTextureSize;
        }
        else
        {
            width = texture.width;
            height = texture.height;
        }

        _swapBuffer = new SwapBuffer(width, height, _initColor);
        Graphics.Blit(texture, _swapBuffer.Current);
        _targetRenderer.sharedMaterials[0].mainTexture = _swapBuffer.Current;

        MeshFilter filter = _target.GetComponent<MeshFilter>();
        _mesh = filter.sharedMesh;
    }

    public void Stamp(Material drawingMat)
    {
        drawingMat.SetTexture(_mainTexId, _swapBuffer.Current);

        RenderTexture temp = RenderTexture.active;

        RenderTexture.active = _swapBuffer.Next;
        GL.Clear(true, true, Color.clear);
        drawingMat.SetPass(0);
        Graphics.DrawMeshNow(_mesh, _target.transform.localToWorldMatrix);

        RenderTexture.active = temp;

        _swapBuffer.Swap();

        SetTexture(_swapBuffer.Current);
    }
}