using System.Collections.Generic;
using System.Linq;

namespace GamepadMapper.Configuration
{
    public class CommandFlag
    {
        public static CommandFlag Parse(string expr)
        {
            return expr.StartsWith("!")
                ? new CommandFlag(expr.Substring(1), true)
                : new CommandFlag(expr, false);
        }

        public CommandFlag(string flag, bool negate)
        {
            Flag = flag;
            Negate = negate;
        }

        public string Flag { get; }

        public bool Negate { get; }

        public override string ToString() => Utils.Escape((Negate ? "!" : "") + Flag);
    }

    public class CommandBinding
    {
        public CommandBinding(string command, IEnumerable<CommandFlag> flags, ActionDescriptor action)
        {
            Command = command;
            Flags = flags?.ToArray() ?? new CommandFlag[0];
            Action = action;
        }

        public string Command { get; }

        public CommandFlag[] Flags { get; }

        public ActionDescriptor Action { get; }

        public override string ToString()
        {
            if (Flags.Length == 0)
            {
                return $"bind {Command} = {Action}";
            }
            else
            {
                return $"bind {Command}({string.Join<CommandFlag>(",", Flags)}) = {Action}";
            }
        }
    }
}
