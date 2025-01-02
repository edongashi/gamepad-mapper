using GamepadMapper.Infrastructure;

namespace GamepadMapper.Actuators
{
    public class FlagMapActuator : IMapping, IClearable
    {
        public FlagMapActuator(FlagCollection flags, string flag)
        {
            Flags = flags;
            Flag = flag;
        }

        public FlagCollection Flags { get; }

        public string Flag { get; }

        public void Activate()
        {
            Flags.Add(Flag);
        }

        public void Deactivate()
        {
            Flags.Remove(Flag);
        }

        public void Clear()
        {
            Deactivate();
        }
    }
}
