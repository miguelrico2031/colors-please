
using UnityEngine;

[System.Serializable]
public struct RGB255
{
    public byte R;
    public byte G;
    public byte B;
    
    private static readonly float MAX_DISTANCE = new Vector3(255f, 255f, 255f).magnitude;

    
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
    
    
    public static float GetSimilarity(RGB255 colorA, RGB255 colorB)
    {
        Vector3 vectorA = new Vector3(colorA.R, colorA.G, colorA.B);
        Vector3 vectorB = new Vector3(colorB.R, colorB.G, colorB.B);
        float distance = Vector3.Distance(vectorA, vectorB);
        float similarity = 1f - distance / MAX_DISTANCE;
        return similarity;
    }

    public static RGB255 operator +(RGB255 a, RGB255 b)
    {
        return new RGB255(
            (byte)Mathf.Clamp(a.R + b.R, 0, 255),
            (byte)Mathf.Clamp(a.G + b.G, 0, 255),
            (byte)Mathf.Clamp(a.B + b.B, 0, 255)
            );
    } 

}
