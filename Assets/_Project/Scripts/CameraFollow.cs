using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float minX = -6f;
    [SerializeField] private float maxX = 35f;
    [SerializeField] private float fixedY = 0f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        var position = transform.position;
        position.x = Mathf.Clamp(target.position.x, minX, maxX);
        position.y = fixedY;
        transform.position = position;
    }
}
