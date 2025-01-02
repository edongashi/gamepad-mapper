using System.Collections.Generic;
using WindowsInput.Native;
using GamepadMapper.Configuration;
using GamepadMapper.Input;
using System.Linq;

namespace GamepadMapper
{
    internal static class Utils
    {
        private static string EscapeQuotes(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        public static string Escape(string value)
        {
            return value.All(x => char.IsLetterOrDigit(x) || x == '!') ? value : $"\"{EscapeQuotes(value)}\"";
        }

        public static List<VirtualKeyCode> ModifiersToKeys(ModifierKeys keys)
        {
            var list = new List<VirtualKeyCode>(3);
            if (keys.HasFlag(ModifierKeys.Control))
            {
                list.Add(VirtualKeyCode.CONTROL);
            }

            if (keys.HasFlag(ModifierKeys.Alt))
            {
                list.Add(VirtualKeyCode.MENU);
            }

            if (keys.HasFlag(ModifierKeys.Shift))
            {
                list.Add(VirtualKeyCode.SHIFT);
            }

            if (keys.HasFlag(ModifierKeys.WinKey))
            {
                list.Add(VirtualKeyCode.LWIN);
            }

            return list;
        }

        public static bool IsCenter(this MenuPosition position)
        {
            return position == MenuPosition.TopCenter
                   || position == MenuPosition.MiddleCenter
                   || position == MenuPosition.BottomCenter;
        }

        public static bool IsMiddle(this MenuPosition position)
        {
            return position == MenuPosition.MiddleLeft
                   || position == MenuPosition.MiddleCenter
                   || position == MenuPosition.MiddleRight;
        }

        public static bool IsTop(this MenuPosition position)
        {
            return position == MenuPosition.TopLeft
                || position == MenuPosition.TopCenter
                || position == MenuPosition.TopRight;
        }

        public static bool IsBottom(this MenuPosition position)
        {
            return position == MenuPosition.BottomLeft
                   || position == MenuPosition.BottomCenter
                   || position == MenuPosition.BottomRight;
        }

        public static bool IsLeft(this MenuPosition position)
        {
            return position == MenuPosition.TopLeft
                || position == MenuPosition.MiddleLeft
                || position == MenuPosition.BottomLeft;
        }

        public static bool IsRight(this MenuPosition position)
        {
            return position == MenuPosition.TopRight
                   || position == MenuPosition.MiddleRight
                   || position == MenuPosition.BottomRight;
        }
    }
}
