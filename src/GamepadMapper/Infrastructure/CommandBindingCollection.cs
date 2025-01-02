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
            public CommandClause(string[] flags, IAction action)
            {
                Flags = flags != null ? new HashSet<string>(flags, StringComparer.OrdinalIgnoreCase) : null;
                Action = action;
            }

            public HashSet<string> Flags { get; }

            public IAction Action { get; }

            public bool Matches(FlagCollection flags)
            {
                return Flags == null || flags.HasExactly(Flags);
            }
        }

        public static CommandBindingCollection FromCollection(
            IEnumerable<CommandBinding> bindings,
            IActionFactory actionFactory)
        {
            var dict = new Dictionary<string, List<CommandClause>>(StringComparer.OrdinalIgnoreCase);
            var unconditionalBindings = new Dictionary<string, ActionDescriptor>(StringComparer.OrdinalIgnoreCase);

            foreach (var binding in bindings)
            {
                if (!dict.ContainsKey(binding.Command))
                {
                    dict[binding.Command] = new List<CommandClause>();
                }

                if (binding.Flags != null)
                {
                    dict[binding.Command].Add(new CommandClause(binding.Flags, actionFactory.Create(binding.Action)));
                }
                else
                {
                    unconditionalBindings[binding.Command] = binding.Action;
                }
            }

            foreach (var pair in unconditionalBindings)
            {
                dict[pair.Key].Add(new CommandClause(null, actionFactory.Create(pair.Value)));
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
