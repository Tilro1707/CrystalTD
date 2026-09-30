using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.2f;
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float endScale = 1f;

    private float timer;

    private void Start()
    {
        transform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / lifetime);

        transform.localScale = Vector3.one * Mathf.Lerp(
            startScale,
            endScale,
            progress
        );

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}