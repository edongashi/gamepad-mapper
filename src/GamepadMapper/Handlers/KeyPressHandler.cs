using GamepadMapper.Actuators;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class KeyPressHandler : IButtonHandler
    {
        private bool isPressed;

        public KeyPressHandler(IAction action)
        {
            Action = action;
        }

        public IAction Action { get; }

        public void ClearState()
        {
            isPressed = false;
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
                }
            }
            else
            {
                isPressed = false;
            }
        }
    }
}
