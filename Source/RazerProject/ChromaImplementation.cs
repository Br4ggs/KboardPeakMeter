using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using RazerProject.COMcomponents.Enums;
using RazerProject.COMcomponents.Interfaces;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;
using ColoreColor = Corale.Colore.Core.Color;

namespace RazerProject
{
    // This is an implementation of the Razer Chroma API using the Colore C# library and COM
    // for getting the necessary audio data
    class ChromaImplementation
    {
        public enum TimerPatterns
        {
            visualiser,
            idle
        }

        public enum VisualiserMode
        {
            Static,
            Random,
            Ramp
        }

        private TimerPatterns _currentPattern;

        /// <summary>
        /// Which pattern should be used for the visualiser timer, currently there are only 2 implementations, idle of actively visualising data
        /// </summary>
        public TimerPatterns CurrentPattern
        {
            get
            {
                return _currentPattern;
            }
            set
            {
                _timer.Stop();
                switch (value)
                {
                    case TimerPatterns.visualiser:
                        {
                            // We swap out the _timer.Elapsed method for the visualiser method
                            _timer.Elapsed -= IdlePattern;
                            _timer.Elapsed += VisualisePattern;
                            // We set the interval time to the specified time in miliseconds
                            _timer.Interval = _timerInterval;
                            // We set the _audioMeter to a source using the given _dataFlowSource
                            _audioMeter = AudioUtils.GetAudioMeterInformation(_dataFlowSource);

                            // If _dataFlowSource points to recording devices we activate them by making a
                            // fake recording which wont be saved, since microphones are deactivated by default
                            // and thus else we wont get any data
                            if(_dataFlowSource == EDataFlow.eCapture && !_isCapturingAudio)
                            {
                                MciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
                                MciSendString("record recsound", "", 0, 0);
                                _isCapturingAudio = true;
                            }

                            // Otherwise we close any sound recording that might be happening at that time
                            else if(_isCapturingAudio)
                            {
                                MciSendString("close recsound ", "", 0, 0);
                                _isCapturingAudio = false;
                            }
                        }
                        break;

                    case TimerPatterns.idle:
                        {
                            // We swap out the _timer.Elapsed method for the idle method
                            _timer.Elapsed -= VisualisePattern;
                            _timer.Elapsed += IdlePattern;
                            _timer.Interval = 1000;
                            _audioMeter = null;

                            // Otherwise we close any sound recording that might be happening at that time
                            if (_dataFlowSource == EDataFlow.eCapture && _isCapturingAudio)
                            {
                                MciSendString("close recsound ", "", 0, 0);
                                _isCapturingAudio = false;
                            }
                        }
                        break;
                }
                _currentPattern = value;
                _timer.Start();
            }
        }

        private VisualiserMode _currentMode;

        /// <summary>
        /// How the incoming data from _audioMeter should be visualised, currently there are 3 implementations:
        /// a static color, a random color or a color based off of a ramp for each button.
        /// </summary>
        public VisualiserMode CurrentMode
        {
            get
            {
                return _currentMode;
            }
            set
            {
                _currentMode = value;
            }
        }

        private EDataFlow _dataFlowSource;

        /// <summary>
        /// The current source for _audioMeter, this is currently either data from the speakers or the microphone
        /// </summary>
        public EDataFlow CurrentSource
        {
            get
            {
                return _dataFlowSource;
            }
            set
            {
                _timer.Stop();
                switch (value)
                {
                    case EDataFlow.eCapture:
                        {
                            _audioMeter = AudioUtils.GetAudioMeterInformation(EDataFlow.eCapture);
                            _dataFlowSource = EDataFlow.eCapture;

                            if(!_isCapturingAudio && _currentPattern == TimerPatterns.visualiser)
                            {
                                MciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
                                MciSendString("record recsound", "", 0, 0);
                                _isCapturingAudio = true;
                            }
                        }
                        break;
                    case EDataFlow.eRender:
                        {
                            _audioMeter = AudioUtils.GetAudioMeterInformation(EDataFlow.eRender);
                            _dataFlowSource = EDataFlow.eRender;

                            if (_isCapturingAudio)
                            {
                                MciSendString("close recsound ", "", 0, 0);
                                _isCapturingAudio = false;
                            }
                        }
                        break;
                }
                _dataFlowSource = value;
                _timer.Start();
            }
        }

        private int _timerInterval = 1000;

        /// <summary>
        /// Interval for keyboard update in miliseconds
        /// </summary>
        public int TimerInterval
        {
            get
            {
                return _timerInterval;
            }
            set
            {
                _timerInterval = value;
                if(_currentPattern == TimerPatterns.visualiser)
                {
                    _timer.Stop();
                    _timer.Interval = _timerInterval;
                    _timer.Start();
                }
            }
        }

        private float _multiplier = 1;

        /// <summary>
        /// Factor to multiply incoming audio value with
        /// </summary>
        public float Multiplier
        {
            get
            {
                return _multiplier;
            }
            set
            {
                _multiplier = value;
            }
        }

        private bool _timerActivated = false;
        
        /// <summary>
        /// Bool value signifying wether the timer is actively running or not
        /// </summary>
        public bool TimerActivated
        {
            get
            {
                return _timerActivated;
            }
        }

        private bool _isCapturingAudio = false;
        private bool _offset = false;
        private IAudioMeterInformation _audioMeter;
        private Timer _timer;
        private ColorPicker _colorPicker;
        private ColoreColor defaultColor = ColoreColor.Green;

        public ChromaImplementation()
        {
            Chroma.Instance.Initialize();
            Chroma.Instance.Keyboard.Clear();
            _timer = new Timer(_timerInterval);
            _colorPicker = new ColorPicker();
        }

        /// <summary>
        /// Method for issuing commands to MCI devices
        /// </summary>
        /// <param name="lpstrCommand">The command to be issued</param>
        /// <param name="lpstrReturnString">Pointer to string that recieves return value</param>
        /// <param name="uReturnLength">Size of return buffer</param>
        /// <param name="hwndCallback">Handle callback window if "notify" flag was specified in the command</param>
        /// <returns>Status code, zero if successful, or an error otherwise</returns>
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        /// <summary>
        /// Methods for converting the ComboBoxItem values to enum values
        /// </summary>
        /// <param name="source">The name of the value in the ComboBoxItem</param>
        public void SetSource(String source)
        {
            switch (source)
            {
                case "From Speakers":
                    CurrentSource = EDataFlow.eRender;
                    break;
                case "From Microphone":
                    CurrentSource = EDataFlow.eCapture;
                    break;
            }
        }

        /// <summary>
        /// Methods for converting the ComboBoxItem values to enum values
        /// </summary>
        /// <param name="source">The name of the value in the ComboBoxItem</param>
        public void SetVisualiserMode(String visualiserMode)
        {
            switch (visualiserMode)
            {
                case "Ramp Color":
                    CurrentMode = VisualiserMode.Ramp;
                    break;
                case "Random Color":
                    CurrentMode = VisualiserMode.Random;
                    break;
                case "Static Color":
                    CurrentMode = VisualiserMode.Static;
                    break;
                default:
                    Console.WriteLine("error");
                    break;
            }
        }

        /// <summary>
        /// The "Visualiser" pattern implementation for the timer, which will result in a
        /// moving visualisation of the data coming in from _audioMeter, where the most right column is
        /// the latest data coming in and all the other keys hold patterns from previous data, oldest
        /// being the most left column
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void VisualisePattern(object source, ElapsedEventArgs e)
        {
            // We get the initial volume level from the COM interface
            float volumeLevel;
            _audioMeter.GetPeakValue(out volumeLevel);

            // We remap the original range to fit to the keyboards rows(that being five rows)W
            float remappedValue = volumeLevel.RemapNumberToNewRange(0, 1, 0, Constants.MaxRows);

            // Then we multiply it by our multiplier, and clamp it to make sure it doesn't exceed the keyboards range
            // We also make sure the number is alrady a whole number, to make casting to an int easier
            remappedValue *= _multiplier;
            float clampedRemappedValue = (float)Math.Ceiling(remappedValue.Clamp(0, Constants.MaxRows));

            // We then reverse the range to fit on the keyboard and cast it to an integer
            int reversedIntegerValue = (int)clampedRemappedValue.RemapNumberToNewRange(0, Constants.MaxRows, Constants.MaxRows, 0);

            for (int c = 0; c < Constants.MaxColumns; c++)
            {
                // Reversed for loop to make sure the keys are lit from bottom to top
                for (int r = (Constants.MaxRows - 1); r > -1; r--)
                {
                    // If this is not a key on the most right column, we copy the value from its neighbour on the right
                    if (c + 1 < Constants.MaxColumns)
                    {
                        Chroma.Instance.Keyboard[r, c] = Chroma.Instance.Keyboard[r, c + 1];
                    }
                    // If this is the most right column, and this row is still within the limit of the data we retreived from _audioMeter,
                    // we light up this key using one of 3 methods, the static color is set on initialisation of the color variable
                    else
                    {
                        if (r >= reversedIntegerValue)
                        {
                            ColoreColor color;
                            switch (_currentMode)
                            {
                                case VisualiserMode.Ramp:
                                    color = _colorPicker.RampColor(r);
                                    break;
                                case VisualiserMode.Random:
                                    color = _colorPicker.RandomColor();
                                    break;
                                default:
                                    color = defaultColor;
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
        }

        /// <summary>
        /// The "Idle" pattern implementation for the timer, which will result in a
        /// moving checker pattern with each lit key being a random color
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void IdlePattern(object source, ElapsedEventArgs e)
        {
            bool skipkey = _offset;
            for (int r = 0; r < Constants.MaxRows; r++)
            {
                // After each column we invert the skipping pattern, creating a checker pattern
                skipkey = !skipkey;
                for (int c = 0; c < Constants.MaxColumns; c++)
                {
                    // If we skipped a key we turn this key on with a random color
                    if (skipkey)
                    {
                        Chroma.Instance.Keyboard[r, c] = _colorPicker.RandomColor();
                        skipkey = false;
                    }
                    // Else we set it to black
                    else
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.Black;
                        skipkey = true;
                    }
                }
            }
            // After the pattern has been rendered we invert the offset, so that the next render
            // will light up the keys that were set to black this render
            _offset = !_offset;
        }
    }


    public static class FloatExtentions
    {
        /// <summary>
        /// Clamps a number inbetween a certain range, if the value should exceed or fall behind the maximum/minimum,
        /// it will be set to either the maximum or minimum
        /// </summary>
        /// <param name="val">The current value</param>
        /// <param name="min">The minimal allowed value</param>
        /// <param name="max">The maximum allowed value</param>
        /// <returns>A number inbetween the maximum and minimal allowed value</returns>
        public static float Clamp(this float val, float min, float max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Remaps a number from an old to a new range
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="minOldRange">The minimum of the old range</param>
        /// <param name="maxOldRange">The maximum of the old range</param>
        /// <param name="minNewRange">The minimum of the new range</param>
        /// <param name="maxNewRange">The maximum of the new range</param>
        /// <returns>A number inbetween the minimum and maximum value of the new range</returns>
        public static float RemapNumberToNewRange(this float value, float minOldRange, float maxOldRange, float minNewRange, float maxNewRange)
        {
            return (value - minOldRange) / (maxOldRange - minOldRange) * (maxNewRange - minNewRange) + minNewRange;
        }
    }
}
