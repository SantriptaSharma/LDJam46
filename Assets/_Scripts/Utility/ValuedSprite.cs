using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ValuedSprite : MonoBehaviour
{
    [Range(1, 5)]
    public int value = 1;
    public Sprite[] sprites;

    SVGImage sr;

    private void Start()
    {
        sr = GetComponent<SVGImage>();
    }

    void Update()
    {
        if (value > sprites.Length) return;
        sr.sprite = sprites[value - 1];
    }
}