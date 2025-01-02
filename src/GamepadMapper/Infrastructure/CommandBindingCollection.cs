using System;
using System.Collections.Generic;
using System.Linq;
using GamepadMapper.Actuators;
using GamepadMapper.Configuration;

namespace GamepadMapper.Infrastructure
{
    public class CommandBindingCollection
    {
        private class CommandClause
        {
            public CommandClause(CommandFlag[] flags, IAction action)
            {
                Flags = flags;
                Action = action;
            }

            public CommandFlag[] Flags { get; }

            public IAction Action { get; }

            public bool Matches(FlagCollection flags)
            {
                return Flags.All(f => f.Negate ? !flags.Has(f.Flag) : flags.Has(f.Flag));
            }
        }

        public static CommandBindingCollection FromCollection(
            IEnumerable<CommandBinding> bindings,
            IActionFactory actionFactory)
        {
            var dict = new Dictionary<string, List<CommandClause>>(StringComparer.OrdinalIgnoreCase);
            foreach (var binding in bindings)
            {
                if (!dict.ContainsKey(binding.Command))
                {
                    dict[binding.Command] = new List<CommandClause>();
                }

                dict[binding.Command].Add(new CommandClause(binding.Flags, actionFactory.Create(binding.Action)));
            }

            return new CommandBindingCollection(dict);
        }

        public static CommandBindingCollection Empty()
        {
            return new CommandBindingCollection(new Dictionary<string, List<CommandClause>>());
        }

        private readonly Dictionary<string, List<CommandClause>> bindings;

        private CommandBindingCollection(Dictionary<string, List<CommandClause>> bindings)
        {
            this.bindings = bindings;
        }

        public bool TryDispatch(string command, FlagCollection flags)
        {
            if (!bindings.TryGetValue(command, out var clauses))
            {
                return false;
            }

            var clause = clauses.FirstOrDefault(c => c.Matches(flags));
            if (clause == null)
            {
                return false;
            }

            try
            {
                clause.Action.Execute();
            }
            catch
            {
                // Ignored...
            }

            return true;
        }
    }
}
