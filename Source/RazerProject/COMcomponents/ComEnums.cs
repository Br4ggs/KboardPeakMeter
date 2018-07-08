using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazerProject.COMcomponents.Enums
{
    /// <summary>
    /// Flags that are used in activation calls to indicate the execution contexts
    /// in which an object is to be run
    /// </summary>
    [Flags]
    internal enum CLSCTX
    {
        CLSCTX_INPROC_SERVER = 0x1,
        CLSCTX_INPROC_HANDLER = 0x2,
        CLSCTX_LOCAL_SERVER = 0x4,
        CLSCTX_REMOTE_SERVER = 0x10,
        CLSCTX_ALL = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER
    }

    /// <summary>
    /// Flags that indicate conditions for creating and deleting
    /// the object and access modes for the object
    /// </summary>
    [Flags]
    internal enum STGM
    {
        STGM_READ = 0x00000000
        // Rest is not implemented
    }

    /// <summary>
    /// defines constants that indicate the direction in which audio data flows
    /// between an audio endpoint device and an application
    /// 
    /// eRender signifies devices which render sound, such as speakers
    /// eCapture signifies devices which capture sound, such as microphones
    /// eAll encapsulates all devices
    /// </summary>
    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
    }

    /// <summary>
    /// Defines constants that indicate the role that the system has
    /// assigned to an audio endpoint device
    /// 
    /// eConsole signifies games, system notification sounds, and voice commands
    /// eMultimedia signifies music, movies, narration, and live music recording
    /// eCommunications signifies voice communications (talking to another person)
    /// </summary>
    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
    }

    /// <summary>
    /// Values that indicate the current state of an audio endpoint device
    /// </summary>
    [Flags]
    internal enum DEVICE_STATE
    {
        ACTIVE = 0x00000001,
        DISABLED = 0x00000002,
        NOTPRESENT = 0x00000004,
        UNPLUGGED = 0x00000008,
        MASK_ALL = 0x0000000F
    }

    /// <summary>
    /// Defines constants that indicate the current state of an audio session
    /// </summary>
    internal enum AudioSessionState
    {
        Inactive = 0,
        Active = 1,
        Expired = 2
    }

    /// <summary>
    /// Defines constants that indicate the current state of an audio device
    /// </summary>
    internal enum AudioDeviceState
    {
        Active = 0x1,
        Disabled = 0x2,
        NotPresent = 0x4,
        Unplugged = 0x8,
    }

    /// <summary>
    /// Defines constants that indicate as to why the audio session what disconnected
    /// </summary>
    internal enum AudioSessionDisconnectReason
    {
        DisconnectReasonDeviceRemoval = 0,
        DisconnectReasonServerShutdown = 1,
        DisconnectReasonFormatChanged = 2,
        DisconnectReasonSessionLogoff = 3,
        DisconnectReasonSessionDisconnected = 4,
        DisconnectReasonExclusiveModeOverride = 5
    }

    /// <summary>
    /// Specifies the variant types.
    /// </summary>
    [Flags]
    internal enum VARTYPE : short
    {
        VT_I4 = 3,
        VT_BOOL = 11,
        VT_UI4 = 19,
        VT_LPWSTR = 31,
        VT_BLOB = 65,
        VT_CLSID = 72,
        // Rest is not implemented
    }
}
