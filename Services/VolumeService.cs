using System;
using NAudio.CoreAudioApi;

namespace ControlPanel.Services {
    public class VolumeService : IDisposable {
        private readonly MMDeviceEnumerator? _enumerator;
        private readonly MMDevice? _device;
        private readonly AudioEndpointVolume? _volumeControl;

        public VolumeService() {
            try {
                _enumerator = new MMDeviceEnumerator();
                _device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                _volumeControl = _device.AudioEndpointVolume;
            }
            catch {
                // Защита на случай, если к ПК не подключены наушники/колонки
            }
        }

        // Считывает системный звук при старте для синхронизации UI
        public int GetCurrentVolume() {
            if (_volumeControl != null) {
                return (int)Math.Round(_volumeControl.MasterVolumeLevelScalar * 100);
            }
            return 50; // значение по умолчанию
        }

        public void SetVolume(int volume) {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            if (_volumeControl != null)
                _volumeControl.MasterVolumeLevelScalar = volume / 100f;
        }

        // Чистим за собой COM-объекты Windows при закрытии программы
        public void Dispose() {
            _volumeControl?.Dispose();
            _device?.Dispose();
            _enumerator?.Dispose();
        }
    }
}