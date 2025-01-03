
using System;
using ColorComparer;

public static class ComparerTool
{
    public const Comparer DefaultComparer = Comparer.LabDeltaE;
    public static float GetSimilarity(RGB255 c1, RGB255 c2, Comparer comparer)
    {
        // return comparer switch
        // {
        //     Comparer.Euclidean => new Euclidean().Similarity(c1, c2),
        //     Comparer.Redmean => new RedMean().Similarity(c1, c2),
        //     Comparer.WeightedEuclidean => new WeightedEuclidean().Similarity(c1, c2),
        //     Comparer.Anton => new Anton().Similarity(c1, c2),
        //     Comparer.LabDeltaE => new LabDeltaE().Similarity(c1, c2),
        //     Comparer.OrderWeightedEuclidean1_4 => new OrderWeighted<Euclidean>(.25f).Similarity(c1, c2),
        //     Comparer.OrderWeightedEuclidean1_2 => new OrderWeighted<Euclidean>(.5f).Similarity(c1, c2),
        //     Comparer.OrderWeightedEuclidean3_4 => new OrderWeighted<Euclidean>(.75f).Similarity(c1, c2),
        //     Comparer.OrderWeightedLabDeltaE1_4 => new OrderWeighted<LabDeltaE>(.25f).Similarity(c1, c2),
        //     Comparer.OrderWeightedLabDeltaE1_2 => new OrderWeighted<LabDeltaE>(.5f).Similarity(c1, c2),
        //     Comparer.OrderWeightedLabDeltaE3_4 => new OrderWeighted<LabDeltaE>(.75f).Similarity(c1, c2),
        //     _ => throw new ArgumentOutOfRangeException()
        // };
        //
        return GetColorComparer(comparer).Similarity(c1, c2);
    }

    public static IColorComparer GetColorComparer(Comparer comparer)
    {
        return comparer switch
        {
            Comparer.Euclidean => new Euclidean(),
            Comparer.Redmean => new RedMean(),
            Comparer.WeightedEuclidean => new WeightedEuclidean(),
            Comparer.Anton => new Anton(),
            Comparer.LabDeltaE => new LabDeltaE(),
            Comparer.OrderWeightedEuclidean1_4 => new OrderWeighted<Euclidean>(.25f),
            Comparer.OrderWeightedEuclidean1_2 => new OrderWeighted<Euclidean>(.5f),
            Comparer.OrderWeightedEuclidean3_4 => new OrderWeighted<Euclidean>(.75f),
            Comparer.OrderWeightedLabDeltaE1_4 => new OrderWeighted<LabDeltaE>(.25f),
            Comparer.OrderWeightedLabDeltaE1_2 => new OrderWeighted<LabDeltaE>(.5f),
            Comparer.OrderWeightedLabDeltaE3_4 => new OrderWeighted<LabDeltaE>(.75f),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}