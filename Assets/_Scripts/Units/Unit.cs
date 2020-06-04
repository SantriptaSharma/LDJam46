using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float maxHealth, attackRate, moveSpeed, attackDamage, range;
    public AudioSource audioSource;
    public float attenuation;
    public int broCost;
    public bool isPlayer;
    [System.NonSerialized]
    public bool isDead = false;
    public bool freeze;
    public int id;

    protected Unit target;
    protected float health;
    protected WarController controller;
    protected int dir;

    protected virtual void Start()
    {
        health = maxHealth;
        dir = isPlayer ? 1 : -1;
        controller = FindObjectOfType<WarController>();
    }

    protected virtual void Update()
    {
        freeze = controller.hasWon || controller.hasLost || target != null || isDead;

        var hits = Physics2D.RaycastAll(transform.position + Vector3.up * 0.5f, Vector2.right * dir, range);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector2.right * dir * range, Color.red);
        var tag = isPlayer ? "EnemyUnit" : "PlayerUnit";
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i].collider;
            if (c.CompareTag(tag))
            {
                target = c.GetComponent<Unit>();
                break;
            }
        }
        if (!freeze)
        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        controller.HandleDeath(this);
        Destroy(gameObject);
    }

    protected virtual void Attack()
    {
        if(audioSource != null)
        {
            float scale = attenuation + 1/(SquaredXDistanceFromCamera(transform.position.x));
            audioSource.PlayOneShot(audioSource.clip, scale);
        }
    }

    public static float SquaredXDistanceFromCamera(float x)
    {
        var dist = Mathf.Pow(Camera.main.transform.position.x, 2) - x * x;
        return dist;
    }

    public virtual void TakeDamage(float damage)
    {
        if (controller.hasLost || controller.hasWon) return;
        health -= damage;
        if (health <= 0) Die();
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        var uOther = other as Unit;
        if (uOther == null) return false;
        return uOther.id == id;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayer)
        {
            if(collision.CompareTag("EnemyTower"))
            {
                controller.DamageEnemyTower(maxHealth);
                Die();
            }
        }
        else
        {
            if(collision.CompareTag("PlayerTower"))
            {
                controller.DamageTower(maxHealth);
                Die();
            }
        }
    }

    public void ApplyStats(UnitStats stats)
    {
        maxHealth = stats.maxHealth;
        attackRate = stats.attackRate;
        moveSpeed = stats.moveSpeed;
        attackDamage = stats.attackDamage;
    }

    public UnitStats GetStats()
    {
        UnitStats stats = new UnitStats();
        stats.maxHealth = maxHealth;
        stats.attackDamage = attackDamage;
        stats.attackRate = attackRate;
        stats.moveSpeed = moveSpeed;
        return stats;
    }
}

[System.Serializable]
public struct UnitStats
{
    public float maxHealth, attackRate, moveSpeed, attackDamage;
    
    public override string ToString()
    {
        return "mh, ms, ar, ad: " + maxHealth.ToString() + "," + moveSpeed.ToString() + "," + attackRate.ToString() + "," + attackDamage.ToString();
    }
}