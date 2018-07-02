using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RazerProject.COMcomponents.Enums;
using RazerProject.COMcomponents.Interfaces;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;
using ColoreColor = Corale.Colore.Core.Color;

namespace RazerProject
{
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

        public enum Source
        {
            Speakers,
            Microphone
        }

        private TimerPatterns _currentPattern;
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
                            _timer.Elapsed -= IdlePattern;
                            _timer.Elapsed += VisualisePattern;
                            _timer.Interval = _timerInterval;
                            _volume = AudioUtils.GetAudioMeterInformation();
                        }
                        break;

                    case TimerPatterns.idle:
                        {
                            _timer.Elapsed -= VisualisePattern;
                            _timer.Elapsed += IdlePattern;
                            _timer.Interval = 1000;
                            _volume = null;
                        }
                        break;
                }
                _currentPattern = value;
                _timer.Start();
            }
        }

        private VisualiserMode _currentMode;
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

        private Source _currentSource;
        public Source CurrentSource
        {
            get
            {
                return _currentSource;
            }
            set
            {
                switch (value)
                {
                    case Source.Microphone:
                        //get audiometer from speaker
                        break;
                    case Source.Speakers:
                        //get audiometer from speakers
                        break;
                }
                _currentSource = value;
                Console.WriteLine(_currentSource);
            }
        }

        private int _timerInterval = 1000;
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
        public float Multiplier { get { return _multiplier; } set { _multiplier = value; } }

        private bool _timerActivated = false;
        public bool TimerActivated { get { return _timerActivated; } }

        private bool _offset = false;
        private IAudioMeterInformation _volume;
        private Timer _timer;
        private ColorPicker _colorPicker;

        public ChromaImplementation()
        {
            Chroma.Instance.Initialize();
            Chroma.Instance.Keyboard.Clear();
            _timer = new Timer(_timerInterval);
            _colorPicker = new ColorPicker();
        }

        // methods for converting ComboBoxItem values to enum values
        public void SetSource(String source)
        {
            switch (source)
            {
                case "From Speakers":
                    CurrentSource = Source.Speakers;
                    break;
                case "From Microphone":
                    CurrentSource = Source.Microphone;
                    break;
            }
        }

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

        public void StartTimer()
        {
            _timer.Start();
            _timerActivated = true;
        }

        public void StopTimer()
        {
            _timer.Stop();
            _timerActivated = false;
        }

        private void VisualisePattern(object source, ElapsedEventArgs e)
        {
            //we get the initial volume level
            float volumeLevel;
            _volume.GetPeakValue(out volumeLevel);

            //we remap the original range to fit the keyboards, and round it off to an integer
            float remappedValue = volumeLevel.RemapNumberToNewRange(0, 1, 0, Constants.MaxRows);

            //multiply it by our multiplier, and clamp it to make sure it doesn't exceed the keyboards range
            remappedValue *= _multiplier;
            float clampedRemappedValue = (float)Math.Ceiling(remappedValue.Clamp(0, Constants.MaxRows));

            //we then reverse the range to fit on the keyboard
            int reversedIntegerValue = (int)clampedRemappedValue.RemapNumberToNewRange(0, Constants.MaxRows, Constants.MaxRows, 0);

            for (int c = 0; c < Constants.MaxColumns; c++)
            {
                for (int r = (Constants.MaxRows - 1); r > -1; r--)
                {
                    if (c + 1 < Constants.MaxColumns)
                    {
                        Chroma.Instance.Keyboard[r, c] = Chroma.Instance.Keyboard[r, c + 1];
                    }
                    else
                    {
                        if (r >= reversedIntegerValue)
                        {
                            // this could be improved later on
                            // like stuffing all the parameters in a payload
                            ColoreColor color = ColoreColor.Green;
                            switch (_currentMode)
                            {
                                case VisualiserMode.Ramp:
                                    color = _colorPicker.RampColor(r);
                                    break;
                                case VisualiserMode.Random:
                                    color = _colorPicker.RandomColor();
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

        private void IdlePattern(object source, ElapsedEventArgs e)
        {
            bool skipkey = _offset;
            for (int r = 0; r < Constants.MaxRows; r++)
            {
                skipkey = !skipkey;
                for (int c = 0; c < Constants.MaxColumns; c++)
                {
                    if (skipkey)
                    {
                        Chroma.Instance.Keyboard[r, c] = _colorPicker.RandomColor();
                        skipkey = false;
                    }
                    else
                    {
                        Chroma.Instance.Keyboard[r, c] = ColoreColor.Black;
                        skipkey = true;
                    }
                }
            }
            _offset = !_offset;
        }
    }

    public static class FloatExtentions
    {
        public static float Clamp(this float val, float min, float max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float RemapNumberToNewRange(this float value, float minOldRange, float maxOldRange, float minNewRange, float maxNewRange)
        {
            return (value - minOldRange) / (maxOldRange - minOldRange) * (maxNewRange - minNewRange) + minNewRange;
        }
    }
}
