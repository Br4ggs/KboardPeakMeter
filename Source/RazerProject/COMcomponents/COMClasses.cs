using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RazerProject.COMcomponents.Classes
{
    /// <summary>
    /// Class imported from COM which allows us to loop through devices which record/emit audio
    /// </summary>
    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator
    {
    }
}
