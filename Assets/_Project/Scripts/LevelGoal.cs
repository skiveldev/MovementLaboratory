using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class LevelGoal : MonoBehaviour
{
    private bool reached;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (reached || other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        reached = true;
        LevelProgress.Instance?.CompleteLevel();
        AudioManager.PlayGoal();
    }
}
