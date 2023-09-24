// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: MathUtils.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;
using System.Linq;
using PCGSharp;

#endregion

public static class MathUtils
{
   //Random utils

    private static Pcg PCGInstance;

    public static Pcg GetPCGInstance()
    {
        return PCGInstance ??= new Pcg(PcgSeed.GuidBasedSeed());
    }
    public static List<ListType> GetRandomElements<ListType>(this List<ListType> list, int num = 1)
    {
        if (num >= list.Count) return list;
        return list.OrderBy(_ => GetPCGInstance().Next()).Take(num).ToList();
    }


    //Interpolation utils (one of which is used for the little hand hover animation) 
    //most of this code is not used 
    //but i watched this GDC talk 'Fast and Funky 1D Nonlinear Transformations' and it was fun to mess around with 
    //https://www.youtube.com/watch?v=mr5xkf6zSzk
    //pretty much unrelated to this project though 

    public delegate float InterpolationFunction(float t, int exponent = 1);

    public static T PowerChain<T>(T x, int exponent) //fast powers -- translated from: http://szhorvat.net/pelican/fast-computation-of-powers.html
    {
        dynamic dx = x;
        switch (exponent)
        {
            case 0: return (T)(object)1;
            case 1: return dx;
        }

        if (exponent % 2 == 0) return PowerChain(dx * dx, exponent / 2);
        if (exponent % 3 == 0) return PowerChain(dx * dx * dx, exponent / 3);
        return PowerChain(dx, exponent - 1) * dx;
    }

    public static T Interpolate<T>(T a, T b, float t)
    {
        dynamic da = a, db = b;
        return da + (db - da) * t;
    }

    public static float SmoothStart(float t, int exponent = 2)
    {
        return PowerChain(t, exponent);
    }

    public static float SmoothStop(float t, int exponent = 2)
    {
        return 1 - PowerChain(1 - t, exponent);
    }

    public static float SmoothStep(float t, int exponent = 2)
    {
        return t < 0.5f ? SmoothStart(t * 2, exponent) / 2 : SmoothStop(t * 2 - 1, exponent) / 2 + 0.5f;
    }

    public static float Mix(InterpolationFunction a, InterpolationFunction b, float weight, float t, int aX = 1,
        int bX = 1)
    {
        return Interpolate(a(t, aX), b(t, bX), weight);
    }

    public static float Crossfade(InterpolationFunction a, InterpolationFunction b, float t)
    {
        return Mix(a, b, t, t);
    }

    public static float InverseCrossfade(InterpolationFunction a, InterpolationFunction b, float t)
    {
        return Mix(a, b, 1 - t, t);
    }

    public static float Scale(InterpolationFunction f, float t, int exponent = 1)
    {
        return t * f(t, exponent);
    }

    public static float InverseScale(InterpolationFunction f, float t, int exponent = 1)
    {
        return (1 - t) * f(t, exponent);
    }

    public static float BlendPower(float t, float power, InterpolationFunction a)
    {
        int decomposedPower = (int)power;
        float remainder = power - decomposedPower;
        return Interpolate(a(t, decomposedPower), a(t, decomposedPower + 1), remainder);
    }

    public static T Interpolate<T>(T a, T b, float t, InterpolationFunction interpolationFunction, int exponent = 1)
    {
        dynamic da = a, db = b;
        return da + (db - da) * interpolationFunction(t, exponent);
    }

    public static float NormalizedBezier3(float b, float c, float t)
    {
        float s = 1 - t;
        float t2 = t * t;
        float s2 = s * s;
        float t3 = t2 * t;
        return 3 * b * s2 * t + 3 * c * s * t2 + t3;
    }

}