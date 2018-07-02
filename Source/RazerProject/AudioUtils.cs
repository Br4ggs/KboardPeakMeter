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
        public static IAudioMeterInformation GetAudioMeterInformation()
        {
            IMMDevice speakers = GetSpeakers();
            if(speakers == null)
            {
                return null;
            }

            object obj;
            if (speakers.Activate(typeof(IAudioMeterInformation).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioMeterInformation;
        }

        public static IAudioEndpointVolume GetMasterVolumeObject()
        {
            IMMDevice speakers = GetSpeakers();
            if(speakers == null)
            {
                return null;
            }

            object obj;
            if (speakers.Activate(typeof(IAudioEndpointVolume).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioEndpointVolume;
        }

        private static IAudioSessionManager2 GetAudioSessionManager()
        {
            IMMDevice speakers = GetSpeakers();
            if(speakers == null)
            {
                return null;
            }

            object obj;
            if (speakers.Activate(typeof(IAudioSessionManager2).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            return obj as IAudioSessionManager2;
        }

        private static IMMDevice GetSpeakers()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
                IMMDevice speakers;
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

                return speakers;
            }
            catch
            {
                return null;
            }
        }
    }
}
