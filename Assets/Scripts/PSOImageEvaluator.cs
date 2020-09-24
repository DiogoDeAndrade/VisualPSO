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
