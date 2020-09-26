using OpenPSO.Lib;
using System;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    public class PerlinLandscape : IFunction
    {
        private int octaves = 8;
        private double amplitude = 20.0;
        private double frequencyX = 0.02;
        private double frequencyY = 0.02;
        private double amplitudePerOctave = 0.5;
        private double frequencyPerOctave = 2.0;
        private double offsetX = 0;
        private double offsetY = 0;

        public PerlinLandscape(int octaves = 8, double amplitude = 20, double amplitudePerOctave = 0.5,
                               double frequencyX = 0.02, double frequencyY = 0.02, double frequencyPerOctave = 2,
                               double offsetX = 0, double offsetY = 0)
        {
            this.octaves = octaves;
            this.amplitude = amplitude;
            this.amplitudePerOctave = amplitudePerOctave;
            this.frequencyX = frequencyX;
            this.frequencyY = frequencyY;
            this.frequencyPerOctave = frequencyPerOctave;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public double Evaluate(IList<double> position)
        {
            if (position.Count != 2)
            {
                throw new ArgumentException(
                    $"{nameof(PerlinLandscape)} function only works in 2D");
            }

            double fitness = 0.0;

            double x = position[0];
            double y = position[1];

            double amp = amplitude;
            double freqX = frequencyX;
            double freqY = frequencyY;

            for (int i = 0; i < octaves; i++)
            {
                fitness += amp * Perlin2d.Evaluate(x * freqX + offsetX, y * freqY + offsetY);
                amp *= amplitudePerOctave;
                freqX *= frequencyPerOctave;
                freqY *= frequencyPerOctave;
            }

            return fitness;
        }
    }
}
