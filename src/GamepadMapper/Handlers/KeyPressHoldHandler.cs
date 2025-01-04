using GamepadMapper.Actuators;
using GamepadMapper.Configuration;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class KeyPressHoldHandler : IButtonHandler
    {
        private bool isPressed;
        private bool holdHandled;
        private double holdDuration;

        public KeyPressHoldHandler(IAction pressAction, IAction holdAction, IHoldConfiguration configuration)
        {
            PressAction = pressAction;
            HoldAction = holdAction;
            Configuration = configuration;
        }

        public IAction PressAction { get; }

        public IAction HoldAction { get; }

        public IHoldConfiguration Configuration { get; }

        public void ClearState()
        {
            isPressed = false;
            holdHandled = false;
            holdDuration = 0d;
            if (PressAction is IClearable pc)
            {
                pc.Clear();
            }

            if (HoldAction is IClearable hc)
            {
                hc.Clear();
            }
        }

        public void Update(ButtonState state, FrameDetails frame)
        {
            if (state.IsPressed)
            {
                if (isPressed)
                {
                    // Down -> down
                    holdDuration += frame.TimeDelta;
                    if (!holdHandled && holdDuration >= Configuration.Duration)
                    {
                        HoldAction.Execute();
                        holdHandled = true;
                    }
                }
                else
                {
                    // Up -> down
                    holdDuration = 0d;
                    isPressed = true;
                }
            }
            else
            {
                if (isPressed)
                {
                    // Down -> up
                    if (holdDuration < Configuration.Duration)
                    {
                        PressAction.Execute();
                    }

                    holdHandled = false;
                    isPressed = false;
                }

                holdDuration = 0d;
            }
        }
    }
}
