using UnityEngine;

namespace ColorComparer
{
    public class WeightedEuclidean : IColorComparer
    {
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));
        
        
        
        public float Distance(RGB255 a, RGB255 b)
        {
            int redMean = (a.R + b.R) / 2;
            
            int deltaR = a.R - b.R;
            int deltaG = a.G - b.G;
            int deltaB = a.G - b.G;

            float redComponent = (redMean < 128 ? 2f : 3f) * deltaR * deltaR;
            float greenComponent = 4 * deltaG * deltaG;
            float blueComponent = (redMean < 128 ? 3f : 2f) * deltaB * deltaB;
            
            float distance = Mathf.Sqrt(redComponent + greenComponent + blueComponent);
            return distance;
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            return 1f - Distance(a, b) / MAX_DISTANCE;
        }
    }
}