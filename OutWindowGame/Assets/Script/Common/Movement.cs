using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
    /// <summary>
    /// 根据角度给与角度方向的力
    /// </summary>
    /// <param name="PlusG">补偿力</param>
    /// <param name="angle">角度</param>
    /// <param name="G">力度</param>
    /// <returns></returns>
    public static Vector3 Angle(float PlusG, float angle, float G)
    {
        Vector3 vector3;
        float eur = angle * Mathf.Deg2Rad;
        float forceX = (float)(PlusG * G * Math.Sin(eur));
        float forceY = (float)(PlusG * G * Math.Cos(eur));
        vector3 = new Vector3(forceX, forceY);
        return vector3;
    }
    /// <summary>
    /// 通过坐标与坐标0的相对角度
    /// </summary>
    /// <param name="Coord"></param>
    /// <returns></returns>
    public static float CoordOutAngle(Vector2 Coord)
    {
        float angle = 0;
        return angle;
    }
}
