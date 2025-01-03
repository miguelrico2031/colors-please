using System;
using UnityEngine;

[System.Serializable]
public struct RGB255 : IEquatable<RGB255>
{
    public byte R;
    public byte G;
    public byte B;


    public static IColorComparer Comparer = ComparerTool.GetColorComparer(ComparerTool.DefaultComparer);

    public RGB255(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        
        
    }

    public RGB255(Color color)
    {
        R = (byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255), 0, 255);
        G = (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255), 0, 255);
        B = (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255), 0, 255);
    }


    public Color ToColor()
    {
        return new Color(R / 255f, G / 255f, B / 255f, 1f);
    }

    public static RGB255 Random()
    {
        return new RGB255(
            (byte)UnityEngine.Random.Range(0, 256),
            (byte)UnityEngine.Random.Range(0, 256),
            (byte)UnityEngine.Random.Range(0, 256));
    }


    public static float GetSimilarity(RGB255 colorA, RGB255 colorB) => Comparer.Similarity(colorA, colorB);

    public enum Coordinate
    {
        RED,
        GREEN,
        BLUE
    }

    public byte GetCoordinate(Coordinate coordinate) => coordinate switch
    {
        Coordinate.RED => R,
        Coordinate.GREEN => G,
        Coordinate.BLUE => B
    };

    public RGB255 SetCoordinate(Coordinate coordinate, byte amount)
    {
        switch (coordinate)
        {
            case Coordinate.RED: R = amount; break;
            case Coordinate.GREEN: G = amount; break;
            case Coordinate.BLUE: B = amount; break;
        }

        return this;
    }

    public bool Equals(RGB255 other)
    {
        return R == other.R && G == other.G && B == other.B;
    }

    public override bool Equals(object obj)
    {
        return obj is RGB255 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }
}