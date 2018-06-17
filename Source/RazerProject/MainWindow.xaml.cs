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
        Timer timer = new Timer(1000);

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

        private void Randombutton_Click(object sender, RoutedEventArgs e)
        {
            IAudioEndpointVolume volume = AudioUtils.GetMasterVolumeObject();
            float volumeLevel;
            volume.GetMasterVolumeLevelScalar(out volumeLevel);
            Console.WriteLine(volumeLevel * 100);
        }

        private void OnTick(object source, ElapsedEventArgs e)
        {
            bool skipkey = offset;
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
            offset = !offset;
        }
    }
}
