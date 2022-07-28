using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static IEnumerator LerpToPosition(Transform transform, Vector3 targetPosition, Quaternion targetRotation,
        float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public static bool CheckPlayerAllowedDirection(Vector3 forward)
    {
        return forward == Vector3.right || forward == Vector3.left;
    }
}
