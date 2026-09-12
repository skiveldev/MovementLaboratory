using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class CoinCollectible : MonoBehaviour
{
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        collected = true;
        LevelProgress.Instance?.CollectCoin();
        AudioManager.PlayCoin();
        gameObject.SetActive(false);
    }
}
