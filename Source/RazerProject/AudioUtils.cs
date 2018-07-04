using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using RazerProject.COMcomponents.Interfaces;
using RazerProject.COMcomponents.Classes;
using RazerProject.COMcomponents.Enums;
using RazerProject.COMcomponents.Structs;

namespace RazerProject
{
    class AudioUtils
    {
        public static IAudioMeterInformation GetAudioMeterInformation(EDataFlow source)
        {
            IMMDevice audioSource = GetAudioSource(source);
            if(audioSource == null)
            {
                return null;
            }

            object obj;
            if (audioSource.Activate(typeof(IAudioMeterInformation).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioMeterInformation;
        }

        public static IAudioEndpointVolume GetMasterVolumeObject(EDataFlow source)
        {
            IMMDevice audioSource = GetAudioSource(source);
            if(audioSource == null)
            {
                return null;
            }

            object obj;
            if (audioSource.Activate(typeof(IAudioEndpointVolume).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioEndpointVolume;
        }

        public static IAudioSessionManager2 GetAudioSessionManager(EDataFlow source)
        {
            IMMDevice audioSource = GetAudioSource(source);
            if(audioSource == null)
            {
                return null;
            }

            object obj;
            if (audioSource.Activate(typeof(IAudioSessionManager2).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioSessionManager2;
        }

        public static IMMDevice GetAudioSource(EDataFlow source)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
                IMMDevice audioSource;
                deviceEnumerator.GetDefaultAudioEndpoint(source, ERole.eMultimedia, out audioSource);

                return audioSource;
            }
            catch
            {
                return null;
            }
        }
    }
}
