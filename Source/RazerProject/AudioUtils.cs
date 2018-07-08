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
    // Utility class for providing COM objects
    class AudioUtils
    {
        /// <summary>
        /// Gets the IAudioMeterInformation COM object of the given dataflow source
        /// </summary>
        /// <param name="source">The source type from which the COM object should be retrieved</param>
        /// <returns>A COM object which inherits IAudioMeterInformation</returns>
        public static IAudioMeterInformation GetAudioMeterInformation(EDataFlow source)
        {
            // We get the current source of audio with the given type
            // If there is none we return null
            IMMDevice audioSource = GetAudioSource(source);
            if(audioSource == null)
            {
                return null;
            }

            // We request an instance of IAudioMeterInformation from the current source, using IAudioMeterInformation's GUID
            // If there is none, or we get a response code other than 0, we return null
            object obj;
            if (audioSource.Activate(typeof(IAudioMeterInformation).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out obj) != 0 || obj == null)
            {
                return null;
            }

            // We cast the given object to IAudioMeterInformation and return it
            return obj as IAudioMeterInformation;
        }

        /// <summary>
        /// Gets the IAudioEndpointVolume COM object of the given dataflow source
        /// </summary>
        /// <param name="source">The source type from which the COM object should be retrieved</param>
        /// <returns>A COM object which inherits IAudioEndpointVolume</returns>
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

        /// <summary>
        /// Gets the IAudioSessionManager2 COM object of the given dataflow source
        /// </summary>
        /// <param name="source">The source type from which the COM object should be retrieved</param>
        /// <returns>A COM object which inherits IAudioSessionManager2</returns>
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

        /// <summary>
        /// Gets the IMMDevice COM object of the given dataflow source
        /// </summary>
        /// <param name="source">The source type from which the COM object should be retrieved</param>
        /// <returns>A COM object which inherits IMMDevice</returns>
        public static IMMDevice GetAudioSource(EDataFlow source)
        {
            try
            {
                // We get an instance of IMMDeviceEnumerator, which will allow us to iterate through
                // the different devices which can record/play audio
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());

                // We get the default audio endpoint for the given source type, with the assigned role
                //of the audio source being multimedia
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
