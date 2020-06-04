using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ValuedSpriteNonSvg : MonoBehaviour
{
    [Range(1, 7)]
    public int value = 1;
    public Sprite[] sprites;

    Image sr;

    private void Start()
    {
        sr = GetComponent<Image>();
    }

    void Update()
    {
        if (value > sprites.Length) return;
        sr.sprite = sprites[value - 1];
    }
}