using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using RazerProject.COMcomponents.Enums;
using RazerProject.COMcomponents.Interfaces;

namespace RazerProject.COMcomponents.Structs
{
    /// <summary>
    /// Specifies the FMTID/PID identifier that programmatically identifies a property. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PROPERTYKEY
    {
        public Guid fmtid;
        public int pid;

        public override string ToString()
        {
            return fmtid.ToString("B") + " " + pid;
        }
    }

    /// <summary>
    /// Union portion of PROPVARIANT
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PROPVARIANTunion
    {
        [FieldOffset(0)]
        public int lVal;
        [FieldOffset(0)]
        public ulong uhVal;
        [FieldOffset(0)]
        public short boolVal;
        [FieldOffset(0)]
        public IntPtr pwszVal;
        [FieldOffset(0)]
        public IntPtr puuid;
        // Rest is not implemented
    }

    /// <summary>
    /// used in the ReadMultiple and WriteMultiple methods of IPropertyStorage to
    /// define the type tag and the value of a property in a property set.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PROPVARIANT
    {
        public VARTYPE vt;
        public ushort wReserved1;
        public ushort wReserved2;
        public ushort wReserved3;
        public PROPVARIANTunion union;

        public object GetValue()
        {
            switch (vt)
            {
                case VARTYPE.VT_BOOL:
                    return union.boolVal != 0;

                case VARTYPE.VT_LPWSTR:
                    return Marshal.PtrToStringUni(union.pwszVal);

                case VARTYPE.VT_UI4:
                    return union.lVal;

                case VARTYPE.VT_CLSID:
                    return (Guid)Marshal.PtrToStructure(union.puuid, typeof(Guid));

                default:
                    return vt.ToString() + ":?";
            }
        }
    }
}
