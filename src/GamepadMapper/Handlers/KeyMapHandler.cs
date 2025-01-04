using GamepadMapper.Actuators;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class KeyMapHandler : IButtonHandler
    {
        private bool isPressed;

        public KeyMapHandler(IMapping mapping)
        {
            Mapping = mapping;
        }

        public IMapping Mapping { get; }

        public void ClearState()
        {
            if (isPressed)
            {
                if (Mapping is IClearable c)
                {
                    c.Clear();
                }

                isPressed = false;
            }
        }

        public void Update(ButtonState state, FrameDetails frame)
        {
            if (state.IsPressed)
            {
                if (!isPressed)
                {
                    Mapping.Activate();
                    isPressed = true;
                }
            }
            else
            {
                if (isPressed)
                {
                    Mapping.Deactivate();
                    isPressed = false;
                }
            }
        }
    }
}
