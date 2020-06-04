using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardRandomiser : MonoBehaviour
{
    [TextArea(4, 5)]
    public string[] textChoices;

    private TypeSlowly t;

    void Start()
    {
        t = GetComponent<TypeSlowly>();
    }

    void Generate()
    {
        int i = Random.Range(0, textChoices.Length);
        t.SetTargetText(textChoices[i]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Generate();
    }
}
