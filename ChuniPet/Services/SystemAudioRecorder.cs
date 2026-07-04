using NAudio.Wave;
using System;
using System.IO;

namespace ChuniPet.Services
{
    public class SystemAudioRecorder
    {
        private WasapiLoopbackCapture? _capture;
        private WaveFileWriter? _writer;
        private string? _outputPath;

        public bool IsRecording { get; private set; }

        public void StartRecording(string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);
            _outputPath = Path.Combine(outputFolder,
                $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

            // Captures the default playback device (speakers/headphones)
            _capture = new WasapiLoopbackCapture();
            _writer = new WaveFileWriter(_outputPath, _capture.WaveFormat);

            _capture.DataAvailable += (s, e) =>
            {
                _writer?.Write(e.Buffer, 0, e.BytesRecorded);
            };

            _capture.RecordingStopped += (s, e) =>
            {
                _writer?.Dispose();
                _writer = null;
                _capture?.Dispose();
                _capture = null;
            };

            _capture.StartRecording();
            IsRecording = true;
        }

        public void StopRecording()
        {
            _capture?.StopRecording();
            IsRecording = false;
        }

        public string? LastRecordingPath => _outputPath;
    }
}