using System;
using System.Collections;
using System.Collections.Generic;

namespace GamepadMapper.Infrastructure
{
    public class FlagCollection
    {
        private readonly Dictionary<string, int> currentFlags = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public bool Has(string flag)
        {
            return currentFlags.TryGetValue(flag, out var value) && value > 0;
        }

        public bool HasExactly(HashSet<string> flags)
        {
            if (flags.Count != currentFlags.Count)
            {
                return false;
            }

            foreach (var flag in flags)
            {
                if (!Has(flag))
                {
                    return false;
                }
            }

            return true;
        }

        public void Add(string flag)
        {
            if (currentFlags.TryGetValue(flag, out var value))
            {
                currentFlags[flag] = value + 1;
            }
            else
            {
                currentFlags[flag] = 1;
            }
        }

        public void Remove(string flag)
        {
            if (currentFlags.TryGetValue(flag, out var value))
            {
                if (value > 1)
                {
                    currentFlags[flag] = value - 1;
                } 
                else
                {
                    currentFlags.Remove(flag);
                }
            }
        }
    }
}
