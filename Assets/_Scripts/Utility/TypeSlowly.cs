using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypeSlowly : MonoBehaviour
{
    [TextArea(4,5)]
    public string targetText;
    public float lettersPerSecond;
    public TextMeshProUGUI textbox;

    private float timeSinceLastWrite;
    private float secondsPerLetter;
    private int nextIndex;

    void Start()
    {
        textbox = GetComponent<TextMeshProUGUI>();
        textbox.text = "";
        nextIndex = 0;
        secondsPerLetter = 1 / lettersPerSecond;
    }

    void Update()
    {
        if (textbox.text == targetText) return;
        if(timeSinceLastWrite >= secondsPerLetter)
        {
            int number = Mathf.FloorToInt(timeSinceLastWrite / secondsPerLetter);
            for(int i = 0; i < number; i++)
            {
                textbox.text += targetText[nextIndex++];
            }
            timeSinceLastWrite = timeSinceLastWrite % secondsPerLetter;
        }
        else
        {
            timeSinceLastWrite += Time.deltaTime;
        }
    }

    public void SetTargetText(string n)
    {
        targetText = n;
        textbox.text = "";
        nextIndex = 0;
    }
}
