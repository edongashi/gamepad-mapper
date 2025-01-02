using System.Collections.Generic;
using System.Linq;
using WindowsInput.Native;
using GamepadMapper.Input;

namespace GamepadMapper.Configuration
{
    public abstract class InputBinding
    {
        public override string ToString() => Stringify();

        public abstract string Stringify();
    }

    public abstract class AnalogBinding : InputBinding
    {
        protected AnalogBinding(ThumbStick thumbStick)
        {
            ThumbStick = thumbStick;
        }

        public ThumbStick ThumbStick { get; }
    }

    public class MouseMapping : AnalogBinding
    {
        public MouseMapping(ThumbStick thumbStick)
            : base(thumbStick)
        {
        }

        public override string Stringify() => (ThumbStick == ThumbStick.Left ? "LS" : "RS") + " = map MOUSE";
    }

    public class RadialMenuMapping : AnalogBinding
    {
        public RadialMenuMapping(ThumbStick thumbStick)
            : base(thumbStick)
        {
        }

        public override string Stringify() => (ThumbStick == ThumbStick.Left ? "LS" : "RS") + " = map RADIAL_MENU";
    }

    public class ScrollMapping : AnalogBinding
    {
        public ScrollMapping(ThumbStick thumbStick)
            : base(thumbStick)
        {
        }

        public override string Stringify() => (ThumbStick == ThumbStick.Left ? "LS" : "RS") + " = map SCROLL";
    }

    public abstract class ButtonBinding : InputBinding
    {
        protected ButtonBinding(InputKey inputKey)
        {
            InputKey = inputKey;
        }

        public InputKey InputKey { get; }
    }

    public class KeyMapping : ButtonBinding
    {
        public KeyMapping(InputKey inputKey, IEnumerable<VirtualKeyCode> keys)
            : base(inputKey)
        {
            Keys = keys?.ToList() ?? new List<VirtualKeyCode>();
        }

        public IReadOnlyList<VirtualKeyCode> Keys { get; }

        public override string Stringify() => $"{InputKey} = map {string.Join("+", Keys)}";
    }

    public class ModMapping : ButtonBinding
    {
        public ModMapping(InputKey inputKey) : base(inputKey)
        {
        }

        public override string Stringify() => $"{InputKey} = map MOD";
    }

    public class FlagMapping : ButtonBinding
    {
        public FlagMapping(InputKey inputKey, string flag) : base(inputKey)
        {
            Flag = flag;
        }

        public string Flag { get; }

        public override string Stringify() => $"{InputKey} = map flag({Utils.Escape(Flag)})";
    }

    public class PressBinding : ButtonBinding
    {
        public PressBinding(InputKey inputKey, ActionDescriptor action, bool repeat)
            : base(inputKey)
        {
            Action = action;
            Repeat = repeat;
        }

        public ActionDescriptor Action { get; }

        public bool Repeat { get; }

        public override string Stringify() => $"{InputKey} = press {Action}" + (Repeat ? " repeat" : "");
    }

    public class PressHoldBinding : ButtonBinding
    {
        public PressHoldBinding(InputKey inputKey, ActionDescriptor pressActionDescriptor, ActionDescriptor holdAction)
            : base(inputKey)
        {
            PressAction = pressActionDescriptor;
            HoldAction = holdAction;
        }

        public ActionDescriptor PressAction { get; }

        public ActionDescriptor HoldAction { get; }

        public override string Stringify() => $"{InputKey} = press {PressAction} hold {HoldAction}";
    }
}
