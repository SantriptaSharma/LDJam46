using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWriter : Unit
{
    public float sineAmplitude, sineFrequency;
    public bool absSine;

    private float yStart;
    string enemyTag, enemyTowerTag;

    protected override void Start()
    {
        base.Start();
        yStart = transform.position.y;
        enemyTag = isPlayer ? "EnemyUnit" : "PlayerUnit";
        enemyTowerTag = isPlayer ? "EnemyTower" : "PlayerTower";
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(enemyTag))
        {
            collision.GetComponent<Unit>().TakeDamage(attackDamage);
        }

        if(collision.CompareTag(enemyTowerTag))
        {
            if (isPlayer)
                controller.DamageEnemyTower(maxHealth);
            else
                controller.DamageTower(maxHealth);
            Die();
        }
    }

    protected override void Update()
    {
        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;
        var p = transform.position;
        var sin = Mathf.Sin(sineFrequency * p.x) * sineAmplitude;
        if (absSine) sin = Mathf.Abs(sin);
        p.y = yStart + sin;
        transform.position = p;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == null) return;
        if (collision.CompareTag(enemyTag))
        {
            collision.GetComponent<Unit>().TakeDamage(attackDamage);
        }
    }
}
