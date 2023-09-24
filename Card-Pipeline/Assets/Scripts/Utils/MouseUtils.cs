// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: MouseUtils.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public static class MouseUtils
{
    public static Vector3 GetMousePosition() 
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.y = 1;
        return mousePosition;
    }
}