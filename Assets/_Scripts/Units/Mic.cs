using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour
{
    public float startRotateSpeed;
    public float rotationTime;
    public float trackTime;
    public float velocity;

    [System.NonSerialized]
    public Unit target;
    [System.NonSerialized]
    public float damage;
    [System.NonSerialized]
    public string eTag;

    private MicState state;
    private float timeTracker, rotationDecay;
    //-2.8
    void Start()
    {
        state = MicState.rotating;
        rotationDecay = startRotateSpeed / rotationTime;
    }

    void AcquireNewTarget()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Sudoku();
            return;
        } 
            
        switch(state)
        {
            case MicState.rotating:
                transform.Rotate(0, 0, startRotateSpeed * Time.deltaTime);
                startRotateSpeed = Mathf.MoveTowards(startRotateSpeed, 0, rotationDecay * Time.deltaTime);
                if (startRotateSpeed == 0) state = MicState.tracking;
                break;

            case MicState.tracking:
                timeTracker += Time.deltaTime;
                if (timeTracker >= trackTime) state = MicState.launched;
                if (target == null) return;
                Vector3 dir = ((target.transform.position+Vector3.up * 0.5f) - transform.position).normalized;
                transform.up = Vector3.Slerp(transform.up, dir, timeTracker/trackTime + 0.15f);
                break;

            case MicState.launched:
                if (target == null) return;
                transform.up = ((target.transform.position+Vector3.up * 0.5f) - transform.position).normalized;
                transform.position += transform.up * velocity * Time.deltaTime;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == null) return;
        if(collision.gameObject == target.gameObject)
        {
            target.TakeDamage(damage);
            Sudoku();
        }
    }

    private void Sudoku()
    {
        Destroy(gameObject);
    }

    enum MicState
    {
        rotating, tracking, launched
    }
}