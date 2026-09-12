using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
[DefaultExecutionOrder(-10000)]
public sealed class ExplodingBarrel : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D contactCollider;
    [SerializeField] private string explosionStateName = "Explode";
    [SerializeField] private AnimationClip explosionClip;

    private bool hasExploded;
    private SpriteRenderer barrelRenderer;
    private SpriteRenderer playerRenderer;

    public bool HasExploded => hasExploded;
    public bool CanExplode => !hasExploded && contactCollider != null && contactCollider.enabled;

    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        contactCollider ??= GetComponent<Collider2D>();
        barrelRenderer = GetComponent<SpriteRenderer>();

        if (contactCollider is BoxCollider2D boxCollider)
        {
            boxCollider.size = new Vector2(0.31f, 0.32f);
            boxCollider.offset = Vector2.zero;
        }

        if (explosionClip == null)
        {
            Debug.LogError($"{name} requires the project-owned barrel controller and explosion clip.", this);
            enabled = false;
            return;
        }

        animator.enabled = false;
    }

    private void Update()
    {
        if (hasExploded || barrelRenderer == null)
        {
            return;
        }

        if (playerRenderer == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            playerRenderer = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
        }

        if (playerRenderer != null && barrelRenderer.bounds.Intersects(playerRenderer.bounds))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (!CanExplode)
        {
            return;
        }

        hasExploded = true;
        contactCollider.enabled = false;
        animator.enabled = true;
        animator.speed = 1f;
        animator.Play(explosionStateName, 0, 0f);
        AudioManager.PlayExplosion();
        StartCoroutine(RemoveAfterExplosion());
    }

    private IEnumerator RemoveAfterExplosion()
    {
        yield return new WaitForSeconds(explosionClip.length / Mathf.Max(animator.speed, 0.01f));
        Destroy(gameObject);
    }
}
