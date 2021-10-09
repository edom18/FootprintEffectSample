using UnityEngine;

public class Stamper : MonoBehaviour
{
    public class PropertyIdDef
    {
        public int ProjectionMatID = 0;
        public int MatrixParamsID = 0;
        public int ProjectTexID = 0;
        public int ViewPosID = 0;

        public PropertyIdDef()
        {
            ProjectionMatID = Shader.PropertyToID("_ProjectionMatrix");
            MatrixParamsID = Shader.PropertyToID("_MatrixParams");
            ProjectTexID = Shader.PropertyToID("_ProjectTex");
            ViewPosID = Shader.PropertyToID("_ViewPos");
        }
    }

    [SerializeField] private Texture2D _stampTexture = null;
    [SerializeField] private OrthoMatrixParam _orthoMatrixParam = default;
    [SerializeField] private GameObject _target = null;
    [SerializeField] private StampDrawer _stampDrawer = null;

    [Header("==== Debug ====")] [SerializeField]
    private bool _showGizoms = true;

    private Matrix4x4 _tMatrix = Matrix4x4.identity;
    private Matrix4x4 _lightView = Matrix4x4.identity;
    private Matrix4x4 _lightProj = Matrix4x4.identity;
    private Matrix4x4 _projectionMatrix = Matrix4x4.identity;
    private PropertyIdDef _propertyIdDef = new PropertyIdDef();

    private Material _targetMaterial = null;
    private Material _previewMaterial = null;
    private Material _stampMaterial = null;

    private void Awake()
    {
        _previewMaterial = new Material(Shader.Find("SimpleStamp/StampProjection"));
        _previewMaterial.EnableKeyword("ORTHOGONAL");
        
        _stampMaterial = new Material(Shader.Find("SimpleStamp/StampDrawer"));
        _stampMaterial.EnableKeyword("ORTHOGONAL");

        Renderer ren = _target.GetComponent<Renderer>();
        _targetMaterial = ren.material;
        ren.materials = new[]
        {
            _targetMaterial,
            _previewMaterial,
        };

        CreateMatrix();
        
        _stampDrawer.Setup();
    }

    private void Update()
    {
        UpdateMatrix();
        UpdateParameters();
    }

    private void OnDestroy()
    {
        Destroy(_previewMaterial);
        Destroy(_stampMaterial);
    }

    private void OnDrawGizmos()
    {
        if (!_showGizoms) return;
        
        Gizmos.color = Color.cyan;

        Vector3 p = transform.position;

        Matrix4x4 mat = Matrix4x4.LookAt(Vector3.zero, transform.forward, transform.up);
        Vector3 blt = mat.MultiplyVector(new Vector3(_orthoMatrixParam.Left, _orthoMatrixParam.Top, _orthoMatrixParam.Near));
        Vector3 blb = mat.MultiplyVector(new Vector3(_orthoMatrixParam.Left, _orthoMatrixParam.Bottom, _orthoMatrixParam.Near));
        Vector3 brt = mat.MultiplyVector(new Vector3(_orthoMatrixParam.Right, _orthoMatrixParam.Top, _orthoMatrixParam.Near));
        Vector3 brb = mat.MultiplyVector(new Vector3(_orthoMatrixParam.Right, _orthoMatrixParam.Bottom, _orthoMatrixParam.Near));

        Vector3 forward = transform.forward * (_orthoMatrixParam.Far - _orthoMatrixParam.Near);
        Vector3 flt = blt + forward;
        Vector3 flb = blb + forward;
        Vector3 frt = brt + forward;
        Vector3 frb = brb + forward;

        // Near clip plane
        Gizmos.DrawLine(p + blt, p + blb);
        Gizmos.DrawLine(p + blt, p + brt);
        Gizmos.DrawLine(p + brt, p + brb);
        Gizmos.DrawLine(p + brb, p + blb);

        // Far clip plane
        Gizmos.DrawLine(p + flt, p + flb);
        Gizmos.DrawLine(p + flt, p + frt);
        Gizmos.DrawLine(p + frt, p + frb);
        Gizmos.DrawLine(p + frb, p + flb);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p + blt, p + flt);
        Gizmos.DrawLine(p + blb, p + flb);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p + brt, p + frt);
        Gizmos.DrawLine(p + brb, p + frb);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }

    private void CreateMatrix()
    {
        _tMatrix = new Matrix4x4(
            new Vector4(0.5f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.5f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
            new Vector4(0.5f, 0.5f, 0.0f, 1.0f)
        );
    }

    private void UpdateMatrix()
    {
        _lightView = Matrix4x4.LookAt(transform.position, transform.position + transform.forward, transform.up).inverse;
        _lightProj = GetLightProjMatrix();
        _lightProj = GL.GetGPUProjectionMatrix(_lightProj, true);
        _projectionMatrix = _tMatrix * _lightProj * _lightView;
    }

    private void UpdateParameters()
    {
        Vector4 matParams = new Vector4(
            _orthoMatrixParam.Near,
            _orthoMatrixParam.Far,
            _orthoMatrixParam.VerticalScale,
            _orthoMatrixParam.HorizontalScale
        );
        _previewMaterial.SetMatrix(_propertyIdDef.ProjectionMatID, _projectionMatrix);
        _previewMaterial.SetVector(_propertyIdDef.MatrixParamsID, matParams);
        _previewMaterial.SetVector(_propertyIdDef.ViewPosID, transform.position);
        _previewMaterial.SetTexture(_propertyIdDef.ProjectTexID, _stampTexture);
        
        _stampMaterial.SetMatrix(_propertyIdDef.ProjectionMatID, _projectionMatrix);
        _stampMaterial.SetVector(_propertyIdDef.MatrixParamsID, matParams);
        _stampMaterial.SetVector(_propertyIdDef.ViewPosID, transform.position);
        _stampMaterial.SetTexture(_propertyIdDef.ProjectTexID, _stampTexture);
    }

    private Matrix4x4 GetLightProjMatrix()
    {
        return Matrix4x4.Ortho(_orthoMatrixParam.Left, _orthoMatrixParam.Right, _orthoMatrixParam.Bottom, _orthoMatrixParam.Top,
            _orthoMatrixParam.Near, _orthoMatrixParam.Far);
    }

    public void Stamp()
    {
        _stampDrawer.Stamp(_stampMaterial);
    }
}