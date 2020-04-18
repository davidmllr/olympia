using System;
using System.Threading;
using DSPLib;
using Game;
using UnityEngine;

namespace Audio.Spectrum
{
    /// <summary>
    /// This controller handles all audio processing within the game.
    /// This class is derived from a class by jesse-scam, where it was called SongController.
    /// It was adapted for my use.
    /// Please find the original here: https://github.com/jesse-scam/algorithmic-beat-mapping-unity/blob/master/Assets/Lib/Internal/SongController.cs
    /// </summary>
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
        /// The clip of the FileHandler is assigned to the AudioSource
        /// </summary>
        private void Awake()
        {
            AudioSource.clip = FileHandler.Instance.clip;
        }

        /// <summary>
        /// Data from the AudioClip is loaded.
        /// Also, the processing starts on a background thread.
        /// </summary>
        private void Start()
        {
            // Preprocess entire audio file upfront
            _analyzer = new SpectralFluxAnalyzer();
            
            var clip = AudioSource.clip;
            /* grab relevant data for preprocessing */
            _multiChannelSamples = new float[clip.samples * clip.channels];
            _numChannels = clip.channels;
            _numTotalSamples = clip.samples;
            _clipLength = clip.length;
            _sampleRate = clip.frequency;

            /* fill sample array with samples from audio file */
            AudioSource.clip.GetData(_multiChannelSamples, 0);

            TileController.Initialize();
            Play();
            
            var bgThread = new Thread(GetAudioSpectrum);

            Debug.Log("<<< Starting Spectrum Analysis >>>");
            bgThread.Start();
            
            GameManager.Instance.SetTimeTotal(clip.length);
            
        }

        /// <summary>
        /// Tilemaps and current time in the UI is updated.
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

        /// <summary>
        /// Tilemaps are updated in the TileController.
        /// </summary>
        private void UpdateTiles()
        {
            var indexToPlot = GetIndexFromTime(AudioSource.time) / 1024;
            TileController.UpdateTiles(_analyzer.SpectralFluxSamples, indexToPlot);
        }

        /// <summary>
        /// Play the attached AudioSource.
        /// </summary>
        public void Play()
        {
            AudioSource.Play();
            isPlaying = true;
        }
        
        
        #region Helpers

        /// <summary>
        /// Get the index of the current sample range by time.
        /// This class was not modified compared to the original.
        /// </summary>
        /// <param name="curTime">Current time in seconds</param>
        /// <returns>Current sample range index</returns>
        private int GetIndexFromTime(float curTime)
        {
            var lengthPerSample = _clipLength / _numTotalSamples;

            return Mathf.FloorToInt(curTime / lengthPerSample);
        }

        /// <summary>
        /// Get the current time in seconds by sample range index.
        /// This class was not modified compared to the original.
        /// </summary>
        /// <param name="index">Current sample range index</param>
        /// <returns>Current time in seconds</returns>
        private float GetTimeFromIndex(int index)
        {
            return 1f / _sampleRate * index;
        }

        /// <summary>
        /// This class is used to process the AudioClip.
        /// It uses FFT to read the samples, afterwards a Spectral Flux Analysis is performed.
        /// This class was not modified compared to the original.
        /// </summary>
        private void GetAudioSpectrum()
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
                    var curSongTime = GetTimeFromIndex(i) * spectrumSampleSize;

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
        
        #endregion
    }
}