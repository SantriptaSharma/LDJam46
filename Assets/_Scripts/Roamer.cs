using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Roamer : MonoBehaviour
{
    public float lowerWalkTime, higherWalkTime;
    public float lowerIdleTime, higherIdleTime;
    public float speed;
    public float min, max;
    public float idleFlipProbability;

    private int dir;
    private States state;
    private float timeCount;
    private float currentTimeLimit;
    private Animator anim;
    private Dictionary<States, StateFunction> stateFunctions;
    private delegate void StateFunction();

    void Start()
    {
        dir = 1;
        anim = GetComponent<Animator>();
        state = States.idle;
        currentTimeLimit = Random.Range(lowerIdleTime, higherIdleTime);
        timeCount = 0;
        stateFunctions = new Dictionary<States, StateFunction>();
        stateFunctions[States.idle] = Idle;
        stateFunctions[States.walking] = Walk;
    }

    void Update()
    {
        stateFunctions[state]();
    }

    void Idle()
    {
        anim.SetBool("walking", false);   
        timeCount += Time.deltaTime;
        var cFlip = idleFlipProbability * Time.deltaTime;
        if (Random.value <= cFlip) Flip();
        if(timeCount >= currentTimeLimit)
        {
            int nDir = Random.Range(0, 2);
            nDir = nDir == 0 ? -1 : 1;
            if (nDir != dir) Flip();
            currentTimeLimit = Random.Range(lowerWalkTime, higherWalkTime);
            timeCount = 0;
            state = States.walking;
        }
    }

    void Walk()
    {
        anim.SetBool("walking", true);
        timeCount += Time.deltaTime;
        var p = transform.position;
        if(dir == 1)
        {
            if (p.x + speed * Time.deltaTime > max) Flip();
        }
        else
        {
            if (p.x - speed * Time.deltaTime < min) Flip();
        }

        p.x += speed * Time.deltaTime * dir;
        transform.position = p;

        if(timeCount >= currentTimeLimit)
        {
            timeCount = 0;
            currentTimeLimit = Random.Range(lowerIdleTime, higherIdleTime);
            state = States.idle;
        }
    }

    void Flip()
    {
        dir *= -1;
        var t = transform.localScale;
        t.x *= -1;
        transform.localScale = t;
    }

    public enum States
    {
        idle, walking
    }
}