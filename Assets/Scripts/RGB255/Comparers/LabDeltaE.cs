
using System;

namespace ColorComparer
{
    public class LabDeltaE : IColorComparer
    {
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));

        public float Distance(RGB255 a, RGB255 b)
        {
            return (float) CalculateColorDifference(a, b);
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            return 1f - Distance(a, b) / MAX_DISTANCE;
        }


        private static double[] RgbToXyz(int r, int g, int b)
        {
            // Normalize RGB values to [0, 1]
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            // Apply sRGB gamma correction
            rNorm = (rNorm <= 0.04045) ? rNorm / 12.92 : Math.Pow((rNorm + 0.055) / 1.055, 2.4);
            gNorm = (gNorm <= 0.04045) ? gNorm / 12.92 : Math.Pow((gNorm + 0.055) / 1.055, 2.4);
            bNorm = (bNorm <= 0.04045) ? bNorm / 12.92 : Math.Pow((bNorm + 0.055) / 1.055, 2.4);

            // Convert to XYZ using the sRGB D65 conversion matrix
            double x = rNorm * 0.4124564 + gNorm * 0.3575761 + bNorm * 0.1804375;
            double y = rNorm * 0.2126729 + gNorm * 0.7151522 + bNorm * 0.0721750;
            double z = rNorm * 0.0193339 + gNorm * 0.1191920 + bNorm * 0.9503041;

            return new double[] { x, y, z };
        }

        private static double[] XyzToLab(double x, double y, double z)
        {
            // Reference white point for D65
            const double refX = 0.95047;
            const double refY = 1.00000;
            const double refZ = 1.08883;

            x /= refX;
            y /= refY;
            z /= refZ;

            // Apply the nonlinear transformation
            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3) : (7.787 * x) + (16.0 / 116);
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3) : (7.787 * y) + (16.0 / 116);
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3) : (7.787 * z) + (16.0 / 116);

            double l = (116 * y) - 16;
            double a = 500 * (x - y);
            double b = 200 * (y - z);

            return new double[] { l, a, b };
        }

        private static double DeltaE(double[] lab1, double[] lab2)
        {
            // Calculate the deltaE (CIE76) between two L*a*b* colors
            return Math.Sqrt(Math.Pow(lab1[0] - lab2[0], 2) +
                             Math.Pow(lab1[1] - lab2[1], 2) +
                             Math.Pow(lab1[2] - lab2[2], 2));
        }

        public static double CalculateColorDifference(RGB255 a, RGB255 b)
        {
            // Convert RGB to XYZ
            double[] xyz1 = RgbToXyz(a.R, a.G, a.B);
            double[] xyz2 = RgbToXyz(b.R, b.G, b.B);

            // Convert XYZ to L*a*b*
            double[] lab1 = XyzToLab(xyz1[0], xyz1[1], xyz1[2]);
            double[] lab2 = XyzToLab(xyz2[0], xyz2[1], xyz2[2]);

            // Calculate deltaE
            return DeltaE(lab1, lab2);
        }
    }

}