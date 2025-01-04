using System;
using GamepadMapper.Actuators;
using GamepadMapper.Configuration;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class KeyPressRepeatHandler : IButtonHandler
    {
        private bool isPressed;
        private double timeLeft;

        public KeyPressRepeatHandler(IAction action, IRepeatConfiguration repeatConfiguration)
        {
            Action = action;
            RepeatConfiguration = repeatConfiguration;
        }

        public IAction Action { get; }

        public IRepeatConfiguration RepeatConfiguration { get; }

        public void ClearState()
        {
            isPressed = false;
            timeLeft = 0d;
            if (Action is IClearable c)
            {
                c.Clear();
            }
        }

        public void Update(ButtonState state, FrameDetails frame)
        {
            if (state.IsPressed)
            {
                if (!isPressed)
                {
                    Action.Execute();
                    isPressed = true;
                    timeLeft = RepeatConfiguration.Delay;
                }
                else
                {
                    var pressure = state.Pressure;
                    var accel = RepeatConfiguration.PressureAcceleration;
                    if (accel != 0d && accel != 1d)
                    {
                        pressure = Math.Pow(pressure, accel);
                    }

                    timeLeft -= frame.TimeDelta * pressure;
                    if (timeLeft <= 0d)
                    {
                        Action.Execute();
                        timeLeft = Math.Max(1d, timeLeft + RepeatConfiguration.Interval);
                    }
                }
            }
            else
            {
                isPressed = false;
            }
        }
    }
}
