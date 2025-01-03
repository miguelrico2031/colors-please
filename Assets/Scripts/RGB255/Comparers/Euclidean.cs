
using UnityEngine;

namespace ColorComparer
{
    public class Euclidean : IColorComparer
    {
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));

        public float Distance(RGB255 a, RGB255 b)
        {
            Vector3 vectorA = new Vector3(a.R, a.G, a.B);
            Vector3 vectorB = new Vector3(b.R, b.G, b.B);
            return Vector3.Distance(vectorA, vectorB);
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            return 1f - Distance(a, b) / MAX_DISTANCE;
        }
    }
}