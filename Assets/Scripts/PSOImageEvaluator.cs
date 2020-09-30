using System;
using System.Collections.Generic;
using UnityEngine;

public class PSOImageSaturationEvaluator : OpenPSO.Lib.IFunction
{
    Texture2D image;
    Rect      bounds;
    double    multiplier;

    public PSOImageSaturationEvaluator(Texture2D image, Rect bounds, double multiplier = 1)
    {
        this.image = image;
        this.bounds = bounds;
        this.multiplier = multiplier;
    }

    public double Evaluate(IList<double> position)
    {
        if (position.Count != 2)
        {
            throw new ArgumentException(
                $"{nameof(PSOImageSaturationEvaluator)} function only works in 2D");
        }

        double x = position[0];
        double y = position[1];

        // Normalize coordinates
        x = (x - bounds.xMin) / bounds.width;
        y = (y - bounds.yMin) / bounds.width;

        var color = image.GetPixelBilinear((float)x, (float)y);

        Color.RGBToHSV(color, out float h, out float s, out float v);

        return s * multiplier;
    }

    public void SetBounds(Rect bounds)
    {
        this.bounds = bounds;
    }
}

public class PSOImageValueEvaluator : OpenPSO.Lib.IFunction
{
    Texture2D image;
    Rect      bounds;
    double    multiplier;

    public PSOImageValueEvaluator(Texture2D image, Rect bounds, double multiplier = 1)
    {
        this.image = image;
        this.bounds = bounds;
        this.multiplier = multiplier;
    }

    public double Evaluate(IList<double> position)
    {
        if (position.Count != 2)
        {
            throw new ArgumentException(
                $"{nameof(PSOImageSaturationEvaluator)} function only works in 2D");
        }

        double x = position[0];
        double y = position[1];

        // Normalize coordinates
        x = (x - bounds.xMin) / bounds.width;
        y = (y - bounds.yMin) / bounds.width;

        var color = image.GetPixelBilinear((float)x, (float)y);

        Color.RGBToHSV(color, out float h, out float s, out float v);

        return v * multiplier;
    }

    public void SetBounds(Rect bounds)
    {
        this.bounds = bounds;
    }
}

public class PSOImageRalphBellCurve : OpenPSO.Lib.IFunction
{
    Texture2D   image;
    Rect        bounds;
    double      multiplier;
    float       dSquared;
    int         radius;
    bool        useMean;
    bool        useStimulus;
    float       s0 = 0.005f;

    public PSOImageRalphBellCurve(Texture2D image, Rect bounds, double multiplier = 1, int radius = 5, bool useMean = true, bool useStimulus = true)
    {
        this.image = image;
        this.bounds = bounds;
        this.multiplier = multiplier;
        this.radius = radius;
        this.useMean = useMean;
        this.useStimulus = useStimulus;

        dSquared = 0.5f;
    }

    public double Evaluate(IList<double> position)
    {
        if (position.Count != 2)
        {
            throw new ArgumentException(
                $"{nameof(PSOImageSaturationEvaluator)} function only works in 2D");
        }

        double x = position[0];
        double y = position[1];

        // Normalize coordinates
        x = (x - bounds.xMin) / bounds.width;
        y = (y - bounds.yMin) / bounds.width;

        x = x * image.width;
        y = y * image.width;

        if (x >= image.width - 1) return 0;
        if (y >= image.height - 1) return 0;

        var mean = ComputeMean((int)x, (int)y, out float weight);

        if (useMean) return mean;

        var variance = ComputeVariance((int)x, (int)y, weight, mean);

        return variance;
    }

    float ComputeMean(int x, int y, out float weight)
    {
        float miu = 0;
        weight = 0;

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (((dx * dx) + (dy * dy)) > (radius * radius)) continue;

                float val = GetValue(x + dx, y + dy);
                miu += val * val;
                weight += val;
            }
        }

        return miu / weight;
    }

    float ComputeVariance(int x, int y, float weight, float mean)
    {
        float variance = 0;

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (((dx * dx) + (dy * dy)) > (radius * radius)) continue;

                float val = GetValue(x + dx, y + dy);
                variance = val * ((val - mean) * (val - mean));
            }
        }

        return variance / weight;
    }

    float GetValue(int x, int y)
    {
        if (useStimulus) return GetStimulus(x, y);

        return GetResponse(x, y);
    }

    float GetStimulus(int x, int y)
    {
        if ((x < 0) || (x >= image.width)) return 0;
        if ((y < 0) || (y >= image.height)) return 0;

        var color0 = image.GetPixel(x, y);
        var color1 = image.GetPixel(x + 1, y + 1);
        var color2 = image.GetPixel(x + 1, y);
        var color3 = image.GetPixel(x, y + 1);

        var gradSquaredR = (Sqr(color0.r - color1.r) + Sqr(color2.r - color3.r)) / dSquared;
        var gradSquaredG = (Sqr(color0.g - color1.g) + Sqr(color2.g - color3.g)) / dSquared;
        var gradSquaredB = (Sqr(color0.b - color1.b) + Sqr(color2.b - color3.b)) / dSquared;

        var S = Mathf.Sqrt(gradSquaredR + gradSquaredG + gradSquaredB);

        return S;
    }

    float GetResponse(int x, int y)
    {
        var s = GetStimulus(x, y);

        if (s == 0) return 0.0f;

        return Mathf.Log(s * s0);
    }

    public void SetBounds(Rect bounds)
    {
        this.bounds = bounds;
    }

    static float Sqr(float v)
    {
        return v * v;
    }
}
