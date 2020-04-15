using System.Collections.Generic;
using UnityEngine;

namespace Audio.Spectrum
{
    public class SpectralFluxInfo
    {
        public bool isPeak;
        public float prunedSpectralFlux;
        public float spectralFlux;
        public float threshold;
        public float time;
    }

    public class SpectralFluxAnalyzer
    {
        private readonly float[] _curSpectrum;
        private readonly int _numSamples = 1024;
        private readonly float[] _prevSpectrum;

        // Sensitivity multiplier to scale the average threshold.
        // In this case, if a rectified spectral flux sample is > 1.5 times the average, it is a peak
        private readonly float _thresholdMultiplier = 1.5f;
        // Number of samples to average in our window
        private const int ThresholdWindowSize = 50;

        private int _indexToProcess;

        public readonly List<SpectralFluxInfo> SpectralFluxSamples;

        public SpectralFluxAnalyzer()
        {
            SpectralFluxSamples = new List<SpectralFluxInfo>();

            // Start processing from middle of first window and increment by 1 from there
            _indexToProcess = ThresholdWindowSize / 2;

            _curSpectrum = new float[_numSamples];
            _prevSpectrum = new float[_numSamples];
        }

        public void setCurSpectrum(float[] spectrum)
        {
            _curSpectrum.CopyTo(_prevSpectrum, 0);
            spectrum.CopyTo(_curSpectrum, 0);
        }

        public void analyzeSpectrum(float[] spectrum, float time)
        {
            // Set spectrum
            setCurSpectrum(spectrum);

            // Get current spectral flux from spectrum
            var curInfo = new SpectralFluxInfo();
            curInfo.time = time;
            curInfo.spectralFlux = calculateRectifiedSpectralFlux();
            SpectralFluxSamples.Add(curInfo);

            // We have enough samples to detect a peak
            if (SpectralFluxSamples.Count >= ThresholdWindowSize)
            {
                // Get Flux threshold of time window surrounding index to process
                SpectralFluxSamples[_indexToProcess].threshold = getFluxThreshold(_indexToProcess);

                // Only keep amp amount above threshold to allow peak filtering
                SpectralFluxSamples[_indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(_indexToProcess);

                // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
                var indexToDetectPeak = _indexToProcess - 1;

                var curPeak = isPeak(indexToDetectPeak);

                if (curPeak) SpectralFluxSamples[indexToDetectPeak].isPeak = true;
                _indexToProcess++;
            }
            else
            {
                Debug.Log(
                    $"Not ready yet.  At spectral flux sample size of {SpectralFluxSamples.Count} growing to {ThresholdWindowSize}");
            }
        }

        private float calculateRectifiedSpectralFlux()
        {
            var sum = 0f;

            // Aggregate positive changes in spectrum data
            for (var i = 0; i < _numSamples; i++) sum += Mathf.Max(0f, _curSpectrum[i] - _prevSpectrum[i]);
            return sum;
        }

        private float getFluxThreshold(int spectralFluxIndex)
        {
            // How many samples in the past and future we include in our average
            var windowStartIndex = Mathf.Max(0, spectralFluxIndex - ThresholdWindowSize / 2);
            var windowEndIndex = Mathf.Min(SpectralFluxSamples.Count - 1, spectralFluxIndex + ThresholdWindowSize / 2);

            // Add up our spectral flux over the window
            var sum = 0f;
            for (var i = windowStartIndex; i < windowEndIndex; i++) sum += SpectralFluxSamples[i].spectralFlux;

            // Return the average multiplied by our sensitivity multiplier
            var avg = sum / (windowEndIndex - windowStartIndex);
            return avg * _thresholdMultiplier;
        }

        private float getPrunedSpectralFlux(int spectralFluxIndex)
        {
            return Mathf.Max(0f,
                SpectralFluxSamples[spectralFluxIndex].spectralFlux - SpectralFluxSamples[spectralFluxIndex].threshold);
        }

        private bool isPeak(int spectralFluxIndex)
        {
            if (SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux >
                SpectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
                SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux >
                SpectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
                return true;
            return false;
        }

        private void logSample(int indexToLog)
        {
            var windowStart = Mathf.Max(0, indexToLog - ThresholdWindowSize / 2);
            var windowEnd = Mathf.Min(SpectralFluxSamples.Count - 1, indexToLog + ThresholdWindowSize / 2);
            Debug.Log(string.Format(
                "Peak detected at song time {0} with pruned flux of {1} ({2} over thresh of {3}).\n" +
                "Thresh calculated on time window of {4}-{5} ({6} seconds) containing {7} samples.",
                SpectralFluxSamples[indexToLog].time,
                SpectralFluxSamples[indexToLog].prunedSpectralFlux,
                SpectralFluxSamples[indexToLog].spectralFlux,
                SpectralFluxSamples[indexToLog].threshold,
                SpectralFluxSamples[windowStart].time,
                SpectralFluxSamples[windowEnd].time,
                SpectralFluxSamples[windowEnd].time - SpectralFluxSamples[windowStart].time,
                windowEnd - windowStart
            ));
        }
    }
}