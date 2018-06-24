using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;
using ColoreColor = Corale.Colore.Core.Color;

using RazerProject.COMcomponents.Enums;
using RazerProject.COMcomponents.Interfaces;

namespace RazerProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String app = "Spotify";
        Random random = new Random();
        bool logging = false;
        bool offset = false;
        Timer timer = new Timer(10);

        float multiplier = 1;

        public MainWindow()
        {
            Chroma.Instance.Initialize();
            Chroma.Instance.Keyboard.Clear();
            InitializeComponent();

            timer.Elapsed += OnTick;
        }

        private void Logbutton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (logging)
            {
                button.Content = "Start the magic";
                logging = false;
                timer.Stop();
                Chroma.Instance.Keyboard.Clear();
            }
            else
            {
                button.Content = "Stop the magic";
                logging = true;
                timer.Start();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            multiplier = float.Parse(box.Text);
        }

        private void Randombutton_Click(object sender, RoutedEventArgs e)
        {
            IAudioEndpointVolume volume = AudioUtils.GetMasterVolumeObject();
            float volumeLevel;
            bool mute;
            volume.GetMute( out mute);

            Console.WriteLine(volume.SetMute(!mute, Guid.Empty));
        }

        private void OnTick(object source, ElapsedEventArgs e)
        {
            IAudioMeterInformation volume = AudioUtils.GetAudioMeterInformation();

            //we get the initial volume level
            float volumeLevel;
            volume.GetPeakValue(out volumeLevel);

            //we remap the original range to fit the keyboards, and round it off to an integer
            float remappedValue = RemapNumberToNewRange(volumeLevel, 0, 1, 0, Constants.MaxRows);

            //multiply it by our multiplier
            remappedValue *= multiplier;
            int intRemappedValue = (int)Math.Ceiling(remappedValue);

            //we then reverse the range to fit on the keyboard
            intRemappedValue = (int)RemapNumberToNewRange(intRemappedValue, 0, Constants.MaxRows, Constants.MaxRows, 0);
            Console.WriteLine(intRemappedValue);

            for(int c = 0; c < Constants.MaxColumns; c++)
            {
                for (int r = (Constants.MaxRows - 1); r > -1; r--)
                {
                    if (c + 1 < Constants.MaxColumns)
                    {
                        Chroma.Instance.Keyboard[r, c] = Chroma.Instance.Keyboard[r, c + 1];
                    }
                    else
                    {
                        if (r >= intRemappedValue)
                        {
                            ColoreColor color = ColoreColor.Green;

                            switch (r)
                            {
                                case 0:
                                    color = ColoreColor.Red;
                                    break;
                                case 1:
                                    color = ColoreColor.Yellow;
                                    break;
                                case 2:
                                case 3:
                                    color = ColoreColor.Orange;
                                    break;
                                default:
                                    color = ColoreColor.Green;
                                    break;
                            }
                            Chroma.Instance.Keyboard[r, c] = color;
                        }
                        else
                        {
                            Chroma.Instance.Keyboard[r, c] = ColoreColor.Black;
                        }
                    }
                }
            }

            //bool clearing = false;
            /*for (int c = 0; c < Constants.MaxColumns; c++)
            {
                if (intRemappedValue >= c)
                {
                    clearing = false;
                }
                else
                {
                    clearing = true;
                }

                for (int r = 0; r < Constants.MaxRows; r++)
                {
                    if (clearing)
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.Black;
                    }
                    else if (!clearing)
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.Green;
                    }
                }
            }*/

            /*bool skipkey = offset;
            for (int r = 0; r < Constants.MaxRows; r++)
            {
                skipkey = !skipkey;
                for (int c = 0; c < Constants.MaxColumns; c++)
                {
                    if (skipkey)
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.White;
                        skipkey = false;
                    }
                    else
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.Black;
                        skipkey = true;
                    }
                }
            }
            offset = !offset;*/
        }

        public float RemapNumberToNewRange(float oldValue, float minOldRange, float maxOldRange, float minNewRange, float maxNewRange)
        {
            return minNewRange + (oldValue - minOldRange) * (maxNewRange - minNewRange) / (maxOldRange - minOldRange);
        }
    }

    public static class ClampExtention
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
