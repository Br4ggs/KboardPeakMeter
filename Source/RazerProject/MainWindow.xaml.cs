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

namespace RazerProject
{
    public partial class MainWindow : Window
    {
        ChromaImplementation chromaImplementation;

        public MainWindow()
        {
            chromaImplementation = new ChromaImplementation();
            chromaImplementation.CurrentPattern = ChromaImplementation.TimerPatterns.idle;
            chromaImplementation.SetVisualiserMode("Static Color");
            InitializeComponent();
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (chromaImplementation.CurrentPattern == ChromaImplementation.TimerPatterns.visualiser)
            {
                button.Content = "Start visualising audio";
                chromaImplementation.CurrentPattern = ChromaImplementation.TimerPatterns.idle;
            }
            else
            {
                button.Content = "Stop visualising audio";
                chromaImplementation.CurrentPattern = ChromaImplementation.TimerPatterns.visualiser;
            }
        }

        private void MultiplySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            chromaImplementation.Multiplier = (float)slider.Value;

            if (slider.Value > 0.95 && slider.Value < 1.05)
            {
                MultiplierValue.Text = "None";
            }
            else
            {
                MultiplierValue.Text = slider.Value.ToString("0.00");
            }
        }

        private void KboardSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            chromaImplementation.TimerInterval = (int)slider.Value;
            KboardUpdateValue.Text = slider.Value.ToString();
        }

        private void FromDataComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem item = comboBox.SelectedValue as ComboBoxItem;
            
            if(item.Content != null)
            {
                chromaImplementation.SetSource(item.Content.ToString());
            }
        }

        private void VisualiseModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem item = comboBox.SelectedValue as ComboBoxItem;

            if(item.Content != null)
            {
                chromaImplementation.SetVisualiserMode(item.Content.ToString());
            }
        }
    }
}
