using UnityEngine;
using System.Collections;

public class Author : Unit
{
    public GameObject line;
    public GameObject linePoint;

    private Animator anim;
    private float attackTimeCounter = 0;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        attackTimeCounter = 0;
    }

    protected override void Update()
    {
        base.Update();
        anim.SetBool("targeted", target != null);
        if (target != null)
        {
            if(attackTimeCounter > attackRate)
            {
                Attack();
                attackTimeCounter = 0;
            }
        }
        attackTimeCounter += Time.deltaTime;
    }

    protected override void Attack()
    {
        base.Attack();
        var lineObject = Instantiate(line, linePoint.transform.position, Quaternion.identity);
        var l = lineObject.GetComponent<AuthorLine>();
        l.enemyTag = isPlayer ? "EnemyUnit" : "PlayerUnit";
        l.damage = attackDamage;
        l.maxLength = range;
        l.timeToMaxLength = 0.3f;
        l.timeToZero = l.timeToMaxLength * 0.4f;
        l.dir = dir;
    }
}
