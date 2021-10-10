using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneController : MonoBehaviour
{
    [SerializeField] private ARPlaneManager _planeManager = null;
    [SerializeField] private Stamper _stamper = null;
    
    private void Start()
    {
        _planeManager.planesChanged += HandlePlanesChanged;
    }

    private void HandlePlanesChanged(ARPlanesChangedEventArgs args)
    {
        _planeManager.planesChanged -= HandlePlanesChanged;
        _stamper.SetTarget(args.added[0].gameObject);
    }
}
