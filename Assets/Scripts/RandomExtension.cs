using UnityEngine;

public static class RandomExtensions
{
    public static int Range(this System.Random gen, int min, int max)
    {
        return gen.Next(min, max);
    }

    public static float Range(this System.Random gen, float min, float max)
    {
        return (float)(gen.NextDouble() * (max - min) + min);
    }

    public static float Gaussian(this System.Random gen, float mean, float stdDev)
    {
        float u1 = (float)gen.NextDouble();
        float u2 = (float)gen.NextDouble();

        var rand_std_normal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                              Mathf.Sin(2.0f * Mathf.PI * u2);

        var rand_normal = mean + stdDev * rand_std_normal;

        return rand_normal;
    }


    public static int Gaussian(this System.Random gen, int mean, int stdDev)
    {
        return (int)gen.Gaussian((float)mean, (float)stdDev);
    }
}
