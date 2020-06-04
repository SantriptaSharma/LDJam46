using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyUIAnimation : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        animator.SetTrigger("opening");
        GameController.instance.OpenStrategy();
    }

    public void Close()
    {
        animator.SetTrigger("closing");
        GameController.instance.CloseStrategy();
    }
}
