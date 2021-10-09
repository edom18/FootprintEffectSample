using UnityEngine;

[System.Serializable]
public class OrthoMatrixParam
{
    public float Near = 0.01f;
    public float Far = 1.0f;
    public float Left = -1f;
    public float Right = 1f;
    public float Bottom = -1f;
    public float Top = 1f;

    public float VerticalScale = 1f;
    public float HorizontalScale = 1f;

    [Tooltip("If this property is true then texture scale will be fixed by ortho projection.")]
    public bool FixScaleOrtho = false;

    public void CopyFrom(OrthoMatrixParam src)
    {
        Near = src.Near;
        Far = src.Far;
        Left = src.Left;
        Right = src.Right;
        Bottom = src.Bottom;
        Top = src.Top;
        VerticalScale = src.VerticalScale;
        HorizontalScale = src.HorizontalScale;
    }

    public void UpdateScale()
    {
        VerticalScale = FixScaleOrtho ? ((Top - Bottom) * 0.5f) : 1f;
        HorizontalScale = FixScaleOrtho ? ((Right - Left) * 0.5f) : 1f;
    }
}