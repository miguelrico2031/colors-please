using UnityEngine;

namespace ColorComparer
{
    public class RedMean : IColorComparer
    {
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));
        
        
        
        public float Distance(RGB255 a, RGB255 b)
        {
            int redMean = (a.R + b.R) / 2;
            
            int deltaR = a.R - b.R;
            int deltaG = a.G - b.G;
            int deltaB = a.G - b.G;

            float redComponent = (2 + redMean / 256f) * deltaR * deltaR;
            float greenComponent = 4 * deltaG * deltaG;
            float blueComponent = (2 + (255 - redMean) / 256f) * deltaB * deltaB;
            
            float distance = Mathf.Sqrt(redComponent + greenComponent + blueComponent);
            return distance;
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            return 1f - Distance(a, b) / MAX_DISTANCE;
        }
    }
}