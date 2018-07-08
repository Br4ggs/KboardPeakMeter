using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Corale.Colore.Core.Color;

namespace RazerProject
{
    /// <summary>
    /// Utility class to help retrieve certain Colore colors
    /// </summary>
    class ColorPicker
    {
        private Random _random;

        public ColorPicker()
        {
            _random = new Random();
        }

        public ColoreColor RandomColor()
        {
            return new ColoreColor(_random.Next(256), _random.Next(256), _random.Next(256));
        }

        public ColoreColor RampColor(int value)
        {
            switch (value)
            {
                case 0:
                    return ColoreColor.Red;
                case 1:
                    return ColoreColor.Orange;
                case 2:
                    return ColoreColor.Yellow;
                case 3:
                    return ColoreColor.Green;
                case 4:
                    return ColoreColor.HotPink;
                case 5:
                    return ColoreColor.Blue;
                default:
                    return ColoreColor.White;
            }
        }
    }
}
