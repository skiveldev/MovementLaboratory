using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class PalmGentleSway : MonoBehaviour
{
    [SerializeField] private Animator importedAnimator;

    private void Awake()
    {
        importedAnimator ??= GetComponent<Animator>();
        DisableAnimation();
    }

    private void OnEnable()
    {
        importedAnimator ??= GetComponent<Animator>();
        DisableAnimation();
    }

    private void DisableAnimation()
    {
        if (importedAnimator != null)
        {
            importedAnimator.speed = 0f;
            importedAnimator.enabled = false;
        }
    }
}
