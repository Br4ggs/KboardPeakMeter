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
        public static IList<AudioSession> GetAllSessions()
        {
            //could this method work with Iaudioclient?
            List<AudioSession> list = new List<AudioSession>();

            IAudioSessionManager2 manager = GetAudioSessionManager();
            if(manager == null)
            {
                return list;
            }

            IAudioSessionEnumerator sessionEnumerator;
            manager.GetSessionEnumerator(out sessionEnumerator);

            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl control;
                sessionEnumerator.GetSession(i, out control);

                if (control == null)
                {
                    continue;
                }

                IAudioSessionControl2 control2 = control as IAudioSessionControl2;
                if(control2 != null)
                {
                    //list.Add(new AudioSession(control2));
                }

            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(manager);
            return list;
        }

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
