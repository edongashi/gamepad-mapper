using System;
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
                currentFlags[flag] = Math.Max(0, value - 1);
            }
        }
    }
}
