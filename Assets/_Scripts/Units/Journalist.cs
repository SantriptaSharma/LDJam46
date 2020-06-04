using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class Journalist : Unit
{
    public GameObject mic;
    
    private Animator anim;
    private float attackTimeCounter;
    private string eTag;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        eTag = isPlayer ? "EnemyUnit" : "PlayerUnit";
    }

    protected override void Update()
    {
        freeze = controller.hasWon || controller.hasLost || target != null || isDead;

        var hits = Physics2D.RaycastAll(transform.position + Vector3.up * 0.5f, Vector2.right * dir, range);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector2.right * dir * range, Color.red);
        List<Unit> enemyUnits = new List<Unit>();
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i].collider;
            if (c.CompareTag(eTag))
            {
                enemyUnits.Add(c.GetComponent<Unit>());
            }
        }

        if (enemyUnits.Count > 0)
        {
            Debug.Log("COUNT: " + enemyUnits.Count.ToString());
            int i = Random.Range(0, enemyUnits.Count);
            Debug.Log(i);
            target = enemyUnits[i];
        }

        if (!freeze)
            transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;
        
        anim.SetBool("targeted", target != null);

        if(target != null)
        {
            if (attackTimeCounter >= attackRate) Attack();
        }

        attackTimeCounter += Time.deltaTime;
    }

    protected override void Attack()
    {
        var m = Instantiate(mic, transform.position + transform.up * 2, Quaternion.identity);
        var o = m.GetComponent<Mic>();
        o.target = target;
        o.damage = attackDamage;
        o.eTag = eTag;
        attackTimeCounter = 0;
    }
}
