using NAudio.CoreAudioApi;
using System.Data;

namespace ControlPanel.Services {
    public class VolumeService {
        public void SetVolume(int volume) {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            try {
                using var enumerator = new MMDeviceEnumerator();
                using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                device.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
            }
            catch {
                // Игнорируем ошибки
            }
        }
    }
}