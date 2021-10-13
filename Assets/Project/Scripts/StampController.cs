using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampController : MonoBehaviour
{
    [SerializeField] private Stamper _stamper = null;
    [SerializeField] private Texture2D[] _textures;
    [SerializeField] private float _height = 0.4f;

    private int _index = 0;

    private void Start()
    {
        _stamper.StampTexture = _textures[0];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateStampPosition();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 250, 50), "足跡"))
        {
            _stamper.Stamp();
        }

        if (GUI.Button(new Rect(270, 10, 250, 50), "変更"))
        {
            Change();
        }
    }

    private void UpdateStampPosition()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;

        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        Vector3 point = hit.point;
        point.y = _height;

        _stamper.transform.position = point;
        Vector3 up = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 forward = Vector3.Cross(Camera.main.transform.right, up);
        _stamper.transform.rotation = Quaternion.LookRotation(forward, up);
    }

    private void Change()
    {
        _index = (_index + 1) % _textures.Length;
        _stamper.StampTexture = _textures[_index];
    }
}