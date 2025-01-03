using UnityEngine;

namespace ColorComparer
{
    public class Anton : IColorComparer
    {
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));

        public float Distance(RGB255 a, RGB255 b)
        {
            return (Mathf.Abs(a.R - b.R) + Mathf.Abs(a.G - b.G) + Mathf.Abs(a.B - b.B)) / 3f;
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            return 1f - Distance(a, b) / MAX_DISTANCE;
        }
    }
}