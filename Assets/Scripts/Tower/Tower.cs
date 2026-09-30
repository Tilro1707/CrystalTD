using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] private float range = 3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Crystals")]
    [SerializeField] private CrystalType slotOne = CrystalType.None;
    [SerializeField] private CrystalType slotTwo = CrystalType.None;

    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Effects")]
    [SerializeField] private float fireAoeRadius = 0.75f;
    [SerializeField] private float lightningChainRange = 2f;
    [SerializeField] private int lightningMaxChains = 2;

    [SerializeField] private SpriteRenderer slotOneRenderer;
    [SerializeField] private SpriteRenderer slotTwoRenderer;

    [SerializeField] private Sprite natureSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite lightningSprite;

    [SerializeField] private Sprite normalProjectileSprite;
    [SerializeField] private Sprite fireProjectileSprite;
    [SerializeField] private Sprite natureProjectileSprite;
    [SerializeField] private Sprite lightningProjectileSprite;

    [SerializeField] private Sprite fireNatureProjectileSprite;
    [SerializeField] private Sprite fireLightningProjectileSprite;
    [SerializeField] private Sprite natureLightningProjectileSprite;

    private void Start()
    {
        UpdateCrystalVisuals();
    }

    public bool HasCrystal(CrystalType type)
    {
        return slotOne == type || slotTwo == type;
    }

    public bool AddCrystal(CrystalType crystal)
    {
        if (slotOne == CrystalType.None)
        {
            slotOne = crystal;
            UpdateCrystalVisuals();
            AudioController.Instance.PlayCrystalInsert();
            return true;
        }

        if (slotTwo == CrystalType.None)
        {
            slotTwo = crystal;
            UpdateCrystalVisuals();
            AudioController.Instance.PlayCrystalInsert();
            return true;
        }

        return false;
    }

    public void Attack(float beatLength, bool isCrit)
    {
        if (slotOne == CrystalType.None && slotTwo == CrystalType.None)
        {
            Shoot(1f, beatLength, normalProjectileSprite, isCrit);
        }
        else if (
            (slotOne == CrystalType.Fire && slotTwo == CrystalType.None) ||
            (slotOne == CrystalType.None && slotTwo == CrystalType.Fire)
        )
        {
            Shoot(
                3f,
                beatLength,
                fireProjectileSprite,
                isCrit,
                hasAoe: true
            );
        }
        else if (
            (slotOne == CrystalType.Nature && slotTwo == CrystalType.None) ||
            (slotOne == CrystalType.None && slotTwo == CrystalType.Nature)
        )
        {
            Shoot(
                2f,
                beatLength,
                natureProjectileSprite,
                isCrit,
                freezes: true,
                freezeCooldownBeats: 3
            );
        }
        else if (
            (slotOne == CrystalType.Lightning && slotTwo == CrystalType.None) ||
            (slotOne == CrystalType.None && slotTwo == CrystalType.Lightning)
        )
        {
            Shoot(
                2f,
                beatLength,
                lightningProjectileSprite,
                isCrit,
                maxChains: lightningMaxChains
            );
        }
        else if (
            slotOne == CrystalType.Fire &&
            slotTwo == CrystalType.Fire
        )
        {
            Shoot(
                5f,
                beatLength,
                fireProjectileSprite,
                isCrit,
                hasAoe: true
            );
        }
        else if (
            slotOne == CrystalType.Nature &&
            slotTwo == CrystalType.Nature
        )
        {
            Shoot(
                2f,
                beatLength,
                natureProjectileSprite,
                isCrit,
                freezes: true,
                freezeCooldownBeats: 2
            );
        }
        else if (
            slotOne == CrystalType.Lightning &&
            slotTwo == CrystalType.Lightning
        )
        {
            Shoot(
                2f,
                beatLength,
                lightningProjectileSprite,
                isCrit,
                maxChains: lightningMaxChains * 2
            );
        }
        else if (
            (slotOne == CrystalType.Fire && slotTwo == CrystalType.Nature) ||
            (slotOne == CrystalType.Nature && slotTwo == CrystalType.Fire)
        )
        {
            Shoot(
                3f,
                beatLength,
                fireNatureProjectileSprite,
                isCrit,
                freezes: true,
                freezeCooldownBeats: 2,
                hasAoe: true
            );
        }
        else if (
            (slotOne == CrystalType.Fire && slotTwo == CrystalType.Lightning) ||
            (slotOne == CrystalType.Lightning && slotTwo == CrystalType.Fire)
        )
        {
            Shoot(
                3f,
                beatLength,
                fireLightningProjectileSprite,
                isCrit,
                hasAoe: true,
                maxChains: lightningMaxChains
            );
        }
        else if (
            (slotOne == CrystalType.Nature && slotTwo == CrystalType.Lightning) ||
            (slotOne == CrystalType.Lightning && slotTwo == CrystalType.Nature)
        )
        {
            Shoot(
                2f,
                beatLength,
                natureLightningProjectileSprite,
                isCrit,
                freezes: true,
                freezeCooldownBeats: 2,
                maxChains: lightningMaxChains
            );
        }
    }

    private void Shoot(
        float baseDamage,
        float beatLength,
        Sprite projectileSprite,
        bool isCrit,
        bool freezes = false,
        int freezeCooldownBeats = 2,
        bool hasAoe = false,
        int maxChains = 0
    )
    {
        Enemy target = FindTarget();

        if (target == null)
            return;

        float damage = isCrit
            ? baseDamage * 1.5f
            : baseDamage;

        AudioController.Instance.PlayShoot(isCrit);

        Projectile projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        projectile.Setup(
            target,
            damage,
            beatLength,
            enemyLayer,
            projectileSprite,
            freezes,
            freezeCooldownBeats,
            hasAoe,
            fireAoeRadius,
            maxChains,
            lightningChainRange
        );
    }

    private Enemy FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            range,
            enemyLayer
        );

        if (hit == null)
            return null;

        return hit.GetComponent<Enemy>();
    }

    public bool IsEmpty()
    {
        return slotOne == CrystalType.None &&
               slotTwo == CrystalType.None;
    }

    private void UpdateCrystalVisuals()
    {
        switch (slotOne)
        {
            case CrystalType.None:
                slotOneRenderer.enabled = false;
                break;
            case CrystalType.Nature:
                slotOneRenderer.sprite = natureSprite;
                slotOneRenderer.enabled = true;
                break;
            case CrystalType.Fire:
                slotOneRenderer.sprite = fireSprite;
                slotOneRenderer.enabled = true;
                break;
            case CrystalType.Lightning:
                slotOneRenderer.sprite = lightningSprite;
                slotOneRenderer.enabled = true;
                break;
        }

        switch (slotTwo)
        {
            case CrystalType.None:
                slotTwoRenderer.enabled = false;
                break;
            case CrystalType.Nature:
                slotTwoRenderer.sprite = natureSprite;
                slotTwoRenderer.enabled = true;
                break;
            case CrystalType.Fire:
                slotTwoRenderer.sprite = fireSprite;
                slotTwoRenderer.enabled = true;
                break;
            case CrystalType.Lightning:
                slotTwoRenderer.sprite = lightningSprite;
                slotTwoRenderer.enabled = true;
                break;
        }
    }
}
