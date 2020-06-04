using UnityEngine;
using System.Collections;

public class AuthorLine : MonoBehaviour
{
    public string enemyTag;
    public float damage;
    public float maxLength;
    public float timeToMaxLength;
    public float timeToZero;
    public int dir;

    private float startLength;
    private float currentLength;
    private bool growing;
    private float growSpeed, shrinkSpeed;
    private int count = 0;

    private void Start()
    {
        growing = true;
        growSpeed = maxLength / timeToMaxLength;
        shrinkSpeed = maxLength / timeToZero;
        startLength = transform.localScale.x;
        currentLength = startLength;
    }

    private void Update()
    {
        var scale = transform.localScale;
        currentLength = scale.x;
        
        if (growing)
        {
            var deltaLength = growSpeed * Time.deltaTime * dir;
            if (Mathf.Abs(currentLength + deltaLength) > maxLength)
            {
                currentLength = maxLength*dir;
                growing = false;
                return;
            }
            currentLength += deltaLength;

        }
        else
        {
            var deltaLength = shrinkSpeed * Time.deltaTime * dir;
            if (dir == 1 && currentLength < 0) Destroy(gameObject);
            if (dir == -1 && currentLength > 0) Destroy(gameObject);
            currentLength -= deltaLength;
        }

        scale.x = currentLength;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(enemyTag))
        {
            collision.GetComponent<Unit>().TakeDamage(damage);
        }
    }
}