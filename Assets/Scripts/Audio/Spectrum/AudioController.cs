using System;
using System.Threading;
using DSPLib;
using Game;
using UnityEngine;

namespace Audio.Spectrum
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : Singleton<AudioController>
    {
        [HideInInspector] public bool ready;
        [HideInInspector] public bool isPlaying;
        
        private float _clipLength;
        private float[] _multiChannelSamples;
        private int _numChannels;
        private int _numTotalSamples;
        private int _sampleRate;
        
        private SpectralFluxAnalyzer _analyzer;
        private TileController TileController => GetComponent<TileController>();
        private AudioSource AudioSource => GetComponent<AudioSource>();

        /// <summary>
        /// </summary>
        private void Awake()
        {
            AudioSource.clip = FileHandler.Instance.clip;
        }

        /// <summary>
        /// </summary>
        private void Start()
        {
            // Preprocess entire audio file upfront
            _analyzer = new SpectralFluxAnalyzer();

            // Need all audio samples.  If in stereo, samples will return with left and right channels interweaved
            // [L,R,L,R,L,R]
            var clip = AudioSource.clip;

            _multiChannelSamples = new float[clip.samples * clip.channels];
            _numChannels = clip.channels;
            _numTotalSamples = clip.samples;
            _clipLength = clip.length;

            // We are not evaluating the audio as it is being played by Unity, so we need the clip's sampling rate
            _sampleRate = clip.frequency;

            AudioSource.clip.GetData(_multiChannelSamples, 0);
            Debug.Log(
                $"Got the following data: numChannels: {_numChannels} / numTotalSamples: {_numTotalSamples} / length: {_clipLength}");


            TileController.Initialize();
            var bgThread = new Thread(getFullSpectrumThreaded);

            Debug.Log("<<< Starting Spectrum Analysis >>>");
            bgThread.Start();
            
            GameManager.Instance.SetTimeTotal(clip.length);
            Play();
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            UpdateTiles();
            GameManager.Instance.SetTimeCurrent(AudioSource.time);

            if (isPlaying && !AudioSource.isPlaying)
            {
                isPlaying = false;
                GameManager.Instance.HandleEndOfGame(false);
            }
        }

        private void UpdateTiles()
        {
            var indexToPlot = getIndexFromTime(AudioSource.time) / 1024;
            TileController.UpdateTiles(_analyzer.SpectralFluxSamples, indexToPlot);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            AudioSource.Play();
            isPlaying = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="curTime"></param>
        /// <returns></returns>
        private int getIndexFromTime(float curTime)
        {
            var lengthPerSample = _clipLength / _numTotalSamples;

            return Mathf.FloorToInt(curTime / lengthPerSample);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private float getTimeFromIndex(int index)
        {
            return 1f / _sampleRate * index;
        }

        /// <summary>
        /// </summary>
        private void getFullSpectrumThreaded()
        {
            try
            {
                // We only need to retain the samples for combined channels over the time domain
                var preProcessedSamples = new float[_numTotalSamples];

                var numProcessed = 0;
                var combinedChannelAverage = 0f;
                for (var i = 0; i < _multiChannelSamples.Length; i++)
                {
                    combinedChannelAverage += _multiChannelSamples[i];

                    // Each time we have processed all channels samples for a point in time, we will store the average of the channels combined
                    if ((i + 1) % _numChannels == 0)
                    {
                        preProcessedSamples[numProcessed] = combinedChannelAverage / _numChannels;
                        numProcessed++;
                        combinedChannelAverage = 0f;
                    }
                }

                Debug.Log("Combine Channels done");
                Debug.Log(preProcessedSamples.Length);

                // Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
                var spectrumSampleSize = 1024;
                var iterations = preProcessedSamples.Length / spectrumSampleSize;

                var fft = new FFT();
                fft.Initialize((uint) spectrumSampleSize);

                Debug.Log($"Processing {iterations} time domain samples for FFT");
                var sampleChunk = new double[spectrumSampleSize];
                for (var i = 0; i < iterations; i++)
                {
                    // Grab the current 1024 chunk of audio sample data
                    Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

                    // Apply our chosen FFT Window
                    var windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint) spectrumSampleSize);
                    var scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
                    var scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

                    // Perform the FFT and convert output (complex numbers) to Magnitude
                    var fftSpectrum = fft.Execute(scaledSpectrumChunk);
                    var scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude(fftSpectrum);
                    scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

                    // These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
                    var curSongTime = getTimeFromIndex(i) * spectrumSampleSize;

                    // Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
                    _analyzer.analyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float) x),
                        curSongTime);
                }

                Debug.Log("<<< Spectrum Analysis finished successfully >>>");
            }
            catch (Exception e)
            {
                // Catch exceptions here since the background thread won't always surface the exception to the main thread
                Debug.Log(e.ToString());
            }
        }
    }
}