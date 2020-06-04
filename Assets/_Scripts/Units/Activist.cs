using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activist : Unit
{
    private float attackTimeCounter;
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        anim.SetBool("targeted", target != null);
        if(target != null)
        {
            attackTimeCounter += Time.deltaTime;
            if (attackTimeCounter >= attackRate)
            {
                Attack();
            }
            if (target == null || target.isDead)
                TargetDied();
        }
        else
        {
            attackTimeCounter = 0;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        attackTimeCounter = 0;
        target.TakeDamage(attackDamage);
    }

    private void TargetDied()
    {
        attackTimeCounter = 0;
    }
}
