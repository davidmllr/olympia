using System.Collections.Generic;
using UnityEngine;

namespace Audio.Spectrum
{
    /// <summary>
    /// This class is a representation of spectral flux features.
    /// It was fully adopted by jesse-scam.
    /// Please find the original here: https://github.com/jesse-scam/algorithmic-beat-mapping-unity/blob/master/Assets/Lib/Internal/SpectralFluxAnalyzer.cs
    /// </summary>
    public class SpectralFluxInfo
    {
        public bool isPeak;
        public float prunedSpectralFlux;
        public float spectralFlux;
        public float threshold;
        public float time;
    }

    /// <summary>
    /// This class is used to calculate the spectral flux of an audio track.
    /// It was fully adopted by jesse-scam.
    /// Please find the original here: https://github.com/jesse-scam/algorithmic-beat-mapping-unity/blob/master/Assets/Lib/Internal/SpectralFluxAnalyzer.cs
    /// </summary>
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

        /// <summary>
        /// Constructor that initializes some fields for the class.
        /// </summary>
        public SpectralFluxAnalyzer()
        {
            SpectralFluxSamples = new List<SpectralFluxInfo>();

            // Start processing from middle of first window and increment by 1 from there
            _indexToProcess = ThresholdWindowSize / 2;

            _curSpectrum = new float[_numSamples];
            _prevSpectrum = new float[_numSamples];
        }

        /// <summary>
        /// Sets the current spectrum as previous spectrum and assigns a new spectrum as current.
        /// </summary>
        /// <param name="spectrum">Spectrum to set as current</param>
        private void setCurSpectrum(float[] spectrum)
        {
            _curSpectrum.CopyTo(_prevSpectrum, 0);
            spectrum.CopyTo(_curSpectrum, 0);
        }

        /// <summary>
        /// Analyzes a given spectrum for spectral flux
        /// </summary>
        /// <param name="spectrum">Provided spectrum</param>
        /// <param name="time">Current time of the song in seconds</param>
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

        /// <summary>
        /// Calculates a rectified spectral flux for the provided spectrums.
        /// </summary>
        /// <returns>The calculated spectral flux</returns>
        private float calculateRectifiedSpectralFlux()
        {
            var sum = 0f;

            // Aggregate positive changes in spectrum data
            for (var i = 0; i < _numSamples; i++) sum += Mathf.Max(0f, _curSpectrum[i] - _prevSpectrum[i]);
            return sum;
        }

        /// <summary>
        /// Calculates the threshold for a given index in the spectrum.
        /// </summary>
        /// <param name="spectralFluxIndex">The spectral flux index</param>
        /// <returns>The threshold for the given index</returns>
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

        /// <summary>
        /// Calculates the pruned spectral flux for a given index in the spectrum.
        /// </summary>
        /// <param name="spectralFluxIndex">The spectral flux index</param>
        /// <returns>The pruned threshold for the given index</returns>
        private float getPrunedSpectralFlux(int spectralFluxIndex)
        {
            return Mathf.Max(0f,
                SpectralFluxSamples[spectralFluxIndex].spectralFlux - SpectralFluxSamples[spectralFluxIndex].threshold);
        }

        /// <summary>
        /// Checks if given spectral flux is a peak.
        /// </summary>
        /// <param name="spectralFluxIndex">Index of the spectral flux to check</param>
        /// <returns>If index within spectral flux is a peak</returns>
        private bool isPeak(int spectralFluxIndex)
        {
            if (SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux >
                SpectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
                SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux >
                SpectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
                return true;
            return false;
        }

        /// <summary>
        /// Logs information about a sample in the spectral flux.
        /// </summary>
        /// <param name="indexToLog">Index of the sample to log</param>
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