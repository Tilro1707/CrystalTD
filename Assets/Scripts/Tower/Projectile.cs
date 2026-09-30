using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float hitDistance = 0.05f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject fireAoeEffectPrefab;

    private Enemy target;
    private float damage;

    private float beatLength;
    private bool freezes;
    private int freezeCooldownBeats;

    private bool hasAoe;
    private float aoeRadius;

    private int maxChains;
    private float chainRange;

    private LayerMask enemyLayer;
    private bool rootSoundPlayed;

    private readonly HashSet<Enemy> hitEnemies = new();

    public void Setup(
        Enemy target,
        float damage,
        float beatLength,
        LayerMask enemyLayer,
        Sprite projectileSprite,
        bool freezes = false,
        int freezeCooldownBeats = 2,
        bool hasAoe = false,
        float aoeRadius = 0f,
        int maxChains = 0,
        float chainRange = 0f
    )
    {
        this.target = target;
        this.damage = damage;
        this.beatLength = beatLength;
        this.enemyLayer = enemyLayer;

        this.freezes = freezes;
        this.freezeCooldownBeats = freezeCooldownBeats;

        this.hasAoe = hasAoe;
        this.aoeRadius = aoeRadius;

        this.maxChains = maxChains;
        this.chainRange = chainRange;

        spriteRenderer.sprite = projectileSprite;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = target.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.transform.position) <= hitDistance)
        {
            Hit();
        }
    }

    private void Hit()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 hitPosition = target.transform.position;

        if (hasAoe)
        {
            if (fireAoeEffectPrefab != null)
            {
                Instantiate(
                    fireAoeEffectPrefab,
                    hitPosition,
                    Quaternion.identity
                );
            }

            AudioController.Instance.PlayFireAoe();
            ApplyAoe(hitPosition);
        }
        else
        {
            ApplyEffect(target);
        }

        if (maxChains > 0 && TryChainFrom(hitPosition))
        {
            return;
        }

        Destroy(gameObject);
    }

    private void ApplyEffect(Enemy enemy)
    {
        if (enemy == null)
            return;

        if (!hitEnemies.Add(enemy))
            return;

        enemy.TakeDamage(damage);

        if (freezes && enemy.Freeze(beatLength, freezeCooldownBeats))
        {
            if (!rootSoundPlayed)
            {
                AudioController.Instance.PlayRoot();
                rootSoundPlayed = true;
            }
        }
    }

    private void ApplyAoe(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            position,
            aoeRadius,
            enemyLayer
        );

        foreach (Collider2D collider in hits)
        {
            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                ApplyEffect(enemy);
            }
        }
    }

    private bool TryChainFrom(Vector2 position)
    {
        Enemy nextEnemy = FindClosestUnhitEnemy(position);

        if (nextEnemy == null)
            return false;

        target = nextEnemy;
        maxChains--;

        return true;
    }

    private Enemy FindClosestUnhitEnemy(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            position,
            chainRange,
            enemyLayer
        );

        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in hits)
        {
            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemy == null || hitEnemies.Contains(enemy))
                continue;

            float distance = Vector2.SqrMagnitude(
                (Vector2)enemy.transform.position - position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
