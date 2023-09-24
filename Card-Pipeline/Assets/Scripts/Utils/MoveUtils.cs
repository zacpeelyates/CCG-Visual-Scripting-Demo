// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: MoveUtils.cs
// Modified: 2023/04/25 @ 01:51

#region

using System.Collections;
using UnityEngine;

#endregion

public static class MoveUtils
{
    public static IEnumerator MoveToTargetPosition(Transform transform, Vector3 targetPosition, float seconds)
    {
        Vector3 startingPosition = transform.position;
        float t = 0;
        while (t < seconds)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, seconds);
            transform.position = MathUtils.Interpolate(startingPosition, targetPosition, t / seconds);
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator MoveToTargetPosition(Transform transform, Vector3 targetPosition, float seconds,
        MathUtils.InterpolationFunction func, int funcExponent)
    {
        Vector3 startingPosition = transform.position;
        float t = 0;
        while (t < seconds)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, seconds);
            transform.position = MathUtils.Interpolate(startingPosition, targetPosition, func(t / seconds, funcExponent));
            yield return new WaitForFixedUpdate();
        }
    }
}