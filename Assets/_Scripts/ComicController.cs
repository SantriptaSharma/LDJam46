using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    public ValuedSpriteNonSvg comic;
    public TypeSlowly storyText;
    public Image image;
    public GameObject clickToContinue;
    [TextArea(4, 6)]
    public string[] story;

    private bool canContinue = false;
    private int index = 1;

    // Start is called before the first frame update
    void Start()
    {
        clickToContinue.SetActive(false);
        comic.value = index;
        storyText.SetTargetText(story[index-1]);
    }

    // Update is called once per frame
    void Update()
    {
        if(storyText.targetText == storyText.textbox.text)
        {
            canContinue = true;
            clickToContinue.SetActive(true);
        }

        if(canContinue && Input.GetMouseButtonDown(0))
        {
            ChangePage();
        }
    }

    void ChangePage()
    { 
        if(index == 7)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            return;
        }
        comic.value = ++index;
        storyText.SetTargetText(story[index-1]);
        canContinue = false;
        clickToContinue.SetActive(false);
    }

    private void LateUpdate()
    {
        image.SetNativeSize();
        if(index == 6)
        {
            image.transform.localScale = new Vector3(0.65f, 0.65f, 1);
        }
        else
        {
            image.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
    }
}
