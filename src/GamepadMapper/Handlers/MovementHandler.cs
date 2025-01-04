using System;
using GamepadMapper.Actuators;
using GamepadMapper.Configuration;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class MovementHandler : IAnalogHandler
    {
        public MovementHandler(IMovementActuator movementActuator, IMovementConfiguration movementConfiguration)
        {
            MovementActuator = movementActuator;
            MovementConfiguration = movementConfiguration;
        }

        public IMovementActuator MovementActuator { get; }

        public IMovementConfiguration MovementConfiguration { get; }

        public void ClearState()
        {
            if (MovementActuator is IClearable c)
            {
                c.Clear();
            }
        }

        public void Update(AnalogState state, FrameDetails frame)
        {
            // Speed is pixels per second
            var speed = MovementConfiguration.Speed * frame.TimeDelta / 1000d;
            var accel = MovementConfiguration.Acceleration;
            double x, y;
            if (accel != 1d)
            {
                var angle = state.Angle;
                var distance = Math.Pow(state.Distance, accel);
                x = distance * Math.Cos(angle);
                y = -distance * Math.Sin(angle);
            }
            else
            {
                x = state.X;
                y = -state.Y;
            }

            if (MovementConfiguration.InvertX)
            {
                x = -x;
            }

            if (MovementConfiguration.InvertY)
            {
                y = -y;
            }

            MovementActuator.Move(x * speed, y * speed);
        }
    }
}
