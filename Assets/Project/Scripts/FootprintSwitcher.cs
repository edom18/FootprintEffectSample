using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootprintSwitcher : MonoBehaviour
{
    [SerializeField] private Stamper _stamper;
    [SerializeField] private Texture2D[] _textures;

    private int _index = 0;

    private void Start()
    {
        _stamper.StampTexture = _textures[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Change();
        }
    }

    private void Change()
    {
        _index = (_index + 1) % _textures.Length;
        _stamper.StampTexture = _textures[_index];
    }
}
