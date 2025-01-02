using System.Collections.Generic;
using System.Linq;

namespace GamepadMapper.Configuration
{
    public class CommandBinding
    {
        public CommandBinding(string command, IEnumerable<string> flags, ActionDescriptor action)
        {
            Command = command;
            Flags = flags?.ToArray();
            Action = action;
        }

        public string Command { get; }

        public string[] Flags { get; }

        public ActionDescriptor Action { get; }

        public override string ToString()
        {
            if (Flags == null)
            {
                return $"bind {Command} = {Action}";
            }
            else
            {
                return $"bind {Command}({string.Join(",", Flags.Select(Utils.Escape))}) = {Action}";
            }
        }
    }
}
