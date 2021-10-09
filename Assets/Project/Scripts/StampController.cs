using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampController : MonoBehaviour
{
    [SerializeField] private Stamper _stamper = null;
    [SerializeField] private float _height = 0.4f;
    
    private void Update()
    {
        UpdateStampPosition();

        if (Input.GetMouseButtonDown(0))
        {
            _stamper.Stamp();
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
}
