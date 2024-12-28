
using UnityEngine;

[System.Serializable]
public struct RGB255
{
    public byte R;
    public byte G;
    public byte B;
    
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

}
