using WindowsInput;

namespace GamepadMapper.Actuators
{
    public class SendCharacterActuator : IAction
    {
        public SendCharacterActuator(char character, IKeyboardSimulator keyboard)
        {
            Character = character;
            Keyboard = keyboard;
        }

        public char Character { get; }

        public IKeyboardSimulator Keyboard { get; }

        public void Execute()
        {
            Keyboard.TextEntry(Character);
        }
    }
}
