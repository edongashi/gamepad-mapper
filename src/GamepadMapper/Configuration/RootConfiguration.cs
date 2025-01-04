using System;
using System.Collections.Generic;
using System.Linq;
using GamepadMapper.Configuration.Parsing;

namespace GamepadMapper.Configuration
{
    public class RootConfiguration
    {
        private static string Escape(string value) => value?.Replace("\"", "\"\"");

        private static double Clamp(double value, double min, double max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        private static bool IsValid(string value, double min, double max)
        {
            var divisor = 1d;
            if (max <= 1d && value.EndsWith("%"))
            {
                value = value.Substring(0, value.Length - 1);
                divisor = 100d;
            }

            return InvariantDouble.TryParse(value, out var result) && result / divisor > min && result / divisor < max;
        }

        private static double Clamp(string value, double min, double max, double def)
        {
            var divisor = 1d;
            if (value.EndsWith("%"))
            {
                value = value.Substring(0, value.Length - 1);
                divisor = 100d;
            }

            return InvariantDouble.TryParse(value, out var result)
                ? Clamp(result / divisor, min, max)
                : def;
        }

        private static double Toggle(double value, params double[] values)
        {
            if (value >= values[values.Length - 1])
            {
                return values[0];
            }

            foreach (var jump in values)
            {
                if (value < jump)
                {
                    return jump;
                }
            }

            return values[values.Length - 1];
        }

        private readonly Dictionary<string, IConfigDescriptor> descriptors
            = new Dictionary<string, IConfigDescriptor>(StringComparer.OrdinalIgnoreCase);

        private double fps = 144d;

        public RootConfiguration()
        {
            const double dzDelta = 0.01d;
            Add(new ConfigDescriptor("deadzone.ls")
            {
                GetValue = () => Deadzone.Ls,
                SetValue = val => Deadzone.Ls = Clamp(val, 0d, 0.99d, 0.1d),
                IsValidValue = val => IsValid(val, 0d, 0.99d),
                FormatValue = () => 100d * Deadzone.Ls + "%",
                Increment = () => Deadzone.Ls = Clamp(Deadzone.Ls + dzDelta, 0d, 0.99d),
                Decrement = () => Deadzone.Ls = Clamp(Deadzone.Ls - dzDelta, 0d, 0.99d),
                Toggle = () => Deadzone.Ls = Toggle(Deadzone.Ls, 0.05d, 0.1d, 0.15d, 0.2d),
                Reset = () => Deadzone.Ls = 0.1d
            });
            Add(new ConfigDescriptor("deadzone.rs")
            {
                GetValue = () => Deadzone.Rs,
                SetValue = val => Deadzone.Rs = Clamp(val, 0d, 0.99d, 0.1d),
                IsValidValue = val => IsValid(val, 0d, 0.99d),
                FormatValue = () => 100d * Deadzone.Rs + "%",
                Increment = () => Deadzone.Rs = Clamp(Deadzone.Rs + dzDelta, 0d, 0.99d),
                Decrement = () => Deadzone.Rs = Clamp(Deadzone.Rs - dzDelta, 0d, 0.99d),
                Toggle = () => Deadzone.Rs = Toggle(Deadzone.Rs, 0.05d, 0.1d, 0.15d, 0.2d),
                Reset = () => Deadzone.Rs = 0.1d
            });
            Add(new ConfigDescriptor("deadzone.lt")
            {
                GetValue = () => Deadzone.Lt,
                SetValue = val => Deadzone.Lt = Clamp(val, 0d, 0.99d, 0.1d),
                IsValidValue = val => IsValid(val, 0d, 0.99d),
                FormatValue = () => 100d * Deadzone.Lt + "%",
                Increment = () => Deadzone.Lt = Clamp(Deadzone.Lt + dzDelta, 0d, 0.99d),
                Decrement = () => Deadzone.Lt = Clamp(Deadzone.Lt - dzDelta, 0d, 0.99d),
                Toggle = () => Deadzone.Lt = Toggle(Deadzone.Lt, 0.05d, 0.1d, 0.15d, 0.2d),
                Reset = () => Deadzone.Lt = 0.1d
            });
            Add(new ConfigDescriptor("deadzone.rt")
            {
                GetValue = () => Deadzone.Rt,
                SetValue = val => Deadzone.Rt = Clamp(val, 0d, 0.99d, 0.1d),
                IsValidValue = val => IsValid(val, 0d, 0.99d),
                FormatValue = () => 100d * Deadzone.Rt + "%",
                Increment = () => Deadzone.Rt = Clamp(Deadzone.Rt + dzDelta, 0d, 0.99d),
                Decrement = () => Deadzone.Rt = Clamp(Deadzone.Rt - dzDelta, 0d, 0.99d),
                Toggle = () => Deadzone.Rt = Toggle(Deadzone.Rt, 0.05d, 0.1d, 0.15d, 0.2d),
                Reset = () => Deadzone.Rt = 0.1d
            });
            Add(new ConfigDescriptor("hold.duration")
            {
                GetValue = () => Hold.Duration,
                SetValue = val => Hold.Duration = Clamp(val, 50d, 3000d, 800d),
                IsValidValue = val => IsValid(val, 50d, 3000d),
                FormatValue = () => Hold.Duration.ToString(),
                Increment = () => Hold.Duration = Clamp(Hold.Duration + 10d, 50d, 3000d),
                Decrement = () => Hold.Duration = Clamp(Hold.Duration - 10d, 50d, 3000d),
                Toggle = () => Hold.Duration = Toggle(Hold.Duration, 600d, 800d, 1000d),
                Reset = () => Hold.Duration = 800d
            });
            Add(new ConfigDescriptor("repeat.delay")
            {
                GetValue = () => Repeat.Delay,
                SetValue = val => Repeat.Delay = Clamp(val, 50d, 3000d, 600d),
                IsValidValue = val => IsValid(val, 50d, 3000d),
                FormatValue = () => Repeat.Delay.ToString(),
                Increment = () => Repeat.Delay = Clamp(Repeat.Delay + 10d, 50d, 3000d),
                Decrement = () => Repeat.Delay = Clamp(Repeat.Delay - 10d, 50d, 3000d),
                Toggle = () => Repeat.Delay = Toggle(Repeat.Delay, 400d, 600d, 800d),
                Reset = () => Repeat.Delay = 600d
            });
            Add(new ConfigDescriptor("repeat.interval")
            {
                GetValue = () => Repeat.Interval,
                SetValue = val => Repeat.Interval = Clamp(val, 25d, 500d, 50d),
                IsValidValue = val => IsValid(val, 25d, 500d),
                FormatValue = () => Repeat.Interval.ToString(),
                Increment = () => Repeat.Interval = Clamp(Repeat.Interval + 5d, 25d, 500d),
                Decrement = () => Repeat.Interval = Clamp(Repeat.Interval - 5d, 25d, 500d),
                Toggle = () => Repeat.Interval = Toggle(Repeat.Interval, 25d, 50d, 100d),
                Reset = () => Repeat.Interval = 50d
            });

            const double MIN_MOUSE_SPEED = 50d;
            const double MAX_MOUSE_SPEED = 5000d;
            const double DEF_MOUSE_SPEED = 400d;
            Add(new ConfigDescriptor("mouse.speed")
            {
                GetValue = () => Mouse.Speed,
                SetValue = val => Mouse.Speed = Clamp(val, MIN_MOUSE_SPEED, MAX_MOUSE_SPEED, DEF_MOUSE_SPEED),
                IsValidValue = val => IsValid(val, MIN_MOUSE_SPEED, MAX_MOUSE_SPEED),
                FormatValue = () => Mouse.Speed.ToString(),
                Increment = () => Mouse.Speed = Clamp(Mouse.Speed + 50d, MIN_MOUSE_SPEED, MAX_MOUSE_SPEED),
                Decrement = () => Mouse.Speed = Clamp(Mouse.Speed - 50d, MIN_MOUSE_SPEED, MAX_MOUSE_SPEED),
                Toggle = () => Mouse.Speed = Toggle(Mouse.Speed, 200d, 400d, 600d, 800d),
                Reset = () => Mouse.Speed = DEF_MOUSE_SPEED
            });

            const double MIN_MOUSE_ACCELERATION = 0d;
            const double MAX_MOUSE_ACCELERATION = 10d;
            const double DEF_MOUSE_ACCELERATION = 1.3d;
            Add(new ConfigDescriptor("mouse.acceleration")
            {
                GetValue = () => Mouse.Acceleration,
                SetValue = val => Mouse.Acceleration = Clamp(val, MIN_MOUSE_ACCELERATION, MAX_MOUSE_ACCELERATION, DEF_MOUSE_ACCELERATION),
                IsValidValue = val => IsValid(val, MIN_MOUSE_ACCELERATION, MAX_MOUSE_ACCELERATION),
                FormatValue = () => Mouse.Acceleration.ToString(),
                Increment = () => Mouse.Acceleration = Clamp(Mouse.Acceleration + 0.1d, MIN_MOUSE_ACCELERATION, MAX_MOUSE_ACCELERATION),
                Decrement = () => Mouse.Acceleration = Clamp(Mouse.Acceleration - 0.1d, MIN_MOUSE_ACCELERATION, MAX_MOUSE_ACCELERATION),
                Toggle = () => Mouse.Acceleration = Toggle(Mouse.Acceleration, 1d, 1.3d, 1.6d),
                Reset = () => Mouse.Acceleration = DEF_MOUSE_ACCELERATION
            });

            Add(new ConfigDescriptor("mouse.invertx")
            {
                GetValue = () => Mouse.InvertX,
                SetValue = val => Mouse.InvertX = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Mouse.InvertX.ToString(),
                Increment = () => Mouse.InvertX = true,
                Decrement = () => Mouse.InvertX = false,
                Toggle = () => Mouse.InvertX = !Mouse.InvertX,
                Reset = () => Mouse.InvertX = false
            });
            Add(new ConfigDescriptor("mouse.inverty")
            {
                GetValue = () => Mouse.InvertY,
                SetValue = val => Mouse.InvertY = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Mouse.InvertY.ToString(),
                Increment = () => Mouse.InvertY = true,
                Decrement = () => Mouse.InvertY = false,
                Toggle = () => Mouse.InvertY = !Mouse.InvertY,
                Reset = () => Mouse.InvertY = false
            });
            Add(new ConfigDescriptor("scroll.speed")
            {
                GetValue = () => Scroll.Speed,
                SetValue = val => Scroll.Speed = Clamp(val, 50d, 3000d, 1500d),
                IsValidValue = val => IsValid(val, 50d, 3000d),
                FormatValue = () => Scroll.Speed.ToString(),
                Increment = () => Scroll.Speed = Clamp(Scroll.Speed + 50d, 200d, 3000d),
                Decrement = () => Scroll.Speed = Clamp(Scroll.Speed - 50d, 200d, 3000d),
                Toggle = () => Scroll.Speed = Toggle(Scroll.Speed, 1000d, 1500d, 2000d),
                Reset = () => Scroll.Speed = 1500d
            });
            Add(new ConfigDescriptor("scroll.acceleration")
            {
                GetValue = () => Scroll.Acceleration,
                SetValue = val => Scroll.Acceleration = Clamp(val, 1d, 2d, 1.5d),
                IsValidValue = val => IsValid(val, 1d, 2d),
                FormatValue = () => Scroll.Acceleration.ToString(),
                Increment = () => Scroll.Acceleration = Clamp(Scroll.Acceleration + 0.1d, 1d, 2d),
                Decrement = () => Scroll.Acceleration = Clamp(Scroll.Acceleration - 0.1d, 1d, 2d),
                Toggle = () => Scroll.Acceleration = Toggle(Scroll.Acceleration, 1d, 1.5d, 2d),
                Reset = () => Scroll.Acceleration = 1.5d
            });
            Add(new ConfigDescriptor("scroll.invertx")
            {
                GetValue = () => Scroll.InvertX,
                SetValue = val => Scroll.InvertX = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Scroll.InvertX.ToString(),
                Increment = () => Scroll.InvertX = true,
                Decrement = () => Scroll.InvertX = false,
                Toggle = () => Scroll.InvertX = !Scroll.InvertX,
                Reset = () => Scroll.InvertX = false
            });
            Add(new ConfigDescriptor("scroll.inverty")
            {
                GetValue = () => Scroll.InvertY,
                SetValue = val => Scroll.InvertY = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Scroll.InvertY.ToString(),
                Increment = () => Scroll.InvertY = true,
                Decrement = () => Scroll.InvertY = false,
                Toggle = () => Scroll.InvertY = !Scroll.InvertY,
                Reset = () => Scroll.InvertY = false
            });
            Add(new ConfigDescriptor("menu.minradius")
            {
                GetValue = () => Menu.MinRadius,
                SetValue = val => Menu.MinRadius = Clamp(val, 0.1d, 0.9d, 0.5d),
                IsValidValue = val => IsValid(val, 0.1d, 0.9d),
                FormatValue = () => 100d * Menu.MinRadius + "%",
                Increment = () => Menu.MinRadius = Clamp(Menu.MinRadius + 0.05d, 0.05d, 0.9d),
                Decrement = () => Menu.MinRadius = Clamp(Menu.MinRadius - 0.05d, 0.05d, 0.9d),
                Toggle = () => Menu.MinRadius = Toggle(Menu.MinRadius, 0.2d, 0.3d, 0.4d, 0.5d),
                Reset = () => Menu.MinRadius = 0.5d
            });
            Add(new ConfigDescriptor("menu.smoothing")
            {
                GetValue = () => Menu.Smoothing,
                SetValue = val => Menu.Smoothing = Clamp(val, 0d, 500d, 100d),
                IsValidValue = val => IsValid(val, 0d, 500d),
                FormatValue = () => Menu.Smoothing.ToString(),
                Increment = () => Menu.Smoothing = Clamp(Menu.Smoothing + 10d, 0d, 500d),
                Decrement = () => Menu.Smoothing = Clamp(Menu.Smoothing - 10d, 0d, 500d),
                Toggle = () => Menu.Smoothing = Toggle(Menu.Smoothing, 0d, 25d, 50d, 100d, 150d),
                Reset = () => Menu.Smoothing = 100d
            });
            Add(new ConfigDescriptor("menu.invertx")
            {
                GetValue = () => Menu.InvertX,
                SetValue = val => Menu.InvertX = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Menu.InvertX.ToString(),
                Increment = () => Menu.InvertX = true,
                Decrement = () => Menu.InvertX = false,
                Toggle = () => Menu.InvertX = !Menu.InvertX,
                Reset = () => Menu.InvertX = false
            });
            Add(new ConfigDescriptor("menu.inverty")
            {
                GetValue = () => Menu.InvertY,
                SetValue = val => Menu.InvertY = bool.TryParse(val, out var b) && b,
                IsValidValue = val => bool.TryParse(val, out _),
                FormatValue = () => Menu.InvertY.ToString(),
                Increment = () => Menu.InvertY = true,
                Decrement = () => Menu.InvertY = false,
                Toggle = () => Menu.InvertY = !Menu.InvertY,
                Reset = () => Menu.InvertY = false
            });
            Add(new ConfigDescriptor("menu.placement")
            {
                GetValue = () => Placement.MenuPosition,
                SetValue = val => Placement.MenuPosition = Enum.TryParse<MenuPosition>(val, true, out var position)
                    ? position
                    : MenuPosition.BottomRight,
                IsValidValue = val => Enum.TryParse<MenuPosition>(val, true, out _),
                FormatValue = () => Placement.MenuPosition.ToString(),
                Increment = () => Placement.MenuPosition = (MenuPosition)(((int)Placement.MenuPosition + 1) % ((int)MenuPosition.BottomRight + 1)),
                Decrement = () => Placement.MenuPosition = (MenuPosition)(((int)Placement.MenuPosition + (int)MenuPosition.BottomRight) % ((int)MenuPosition.BottomRight + 1)),
                Toggle = () => Placement.MenuPosition = (MenuPosition)(((int)Placement.MenuPosition + 1) % ((int)MenuPosition.BottomRight + 1)),
                Reset = () => Placement.MenuPosition = MenuPosition.BottomRight
            });
            Add(new ConfigDescriptor("menu.scale")
            {
                GetValue = () => Placement.Scale,
                SetValue = val => Placement.Scale = Clamp(val, 0.5d, 10d, 1d),
                IsValidValue = val => IsValid(val, 0.5d, 10d),
                FormatValue = () => Placement.Scale.ToString(),
                Increment = () => Placement.Scale = Clamp(Placement.Scale + 0.1d, 0.5d, 10d),
                Decrement = () => Placement.Scale = Clamp(Placement.Scale - 0.1d, 0.5d, 10d),
                Toggle = () => Placement.Scale = Toggle(Placement.Scale, 1d, 2d, 3d, 4d),
                Reset = () => Placement.Scale = 1d
            });
            Add(new ConfigDescriptor("profile.current")
            {
                GetValue = () => CurrentProfile,
                SetValue = val => CurrentProfile = val,
                FormatValue = () => CurrentProfile,
                Increment = () => CurrentProfile = GetNextProfile(CurrentProfile),
                Decrement = () => CurrentProfile = GetPreviousProfile(CurrentProfile),
                Toggle = () => CurrentProfile = GetNextProfile(CurrentProfile),
                Reset = () => CurrentProfile = InitialProfile
            });
        }

        public void InitializeProfiles()
        {
            if (CurrentProfile == null || string.Equals(CurrentProfile, "menu", StringComparison.OrdinalIgnoreCase))
            {
                CurrentProfile = "Default";
            }

            if (!Profiles.Any(p => string.Equals(p.Name, CurrentProfile, StringComparison.OrdinalIgnoreCase)))
            {
                CurrentProfile = Profiles.FirstOrDefault()?.Name;
            }

            InitialProfile = CurrentProfile ?? "Default";
        }

        public double Fps
        {
            get => fps;
            set
            {
                if (value < 1d)
                {
                    fps = 1d;
                }
                else if (value > 300d)
                {
                    fps = 300d;
                }
                else
                {
                    fps = value;
                }
            }
        }

        public IReadOnlyDictionary<string, IConfigDescriptor> Descriptors => descriptors;

        public DeadzoneConfiguration Deadzone { get; } = new DeadzoneConfiguration();

        public HoldConfiguration Hold { get; } = new HoldConfiguration();

        public RepeatConfiguration Repeat { get; } = new RepeatConfiguration();

        public MovementConfiguration Mouse { get; } = new MovementConfiguration();

        public MovementConfiguration Scroll { get; } = new MovementConfiguration();

        public RadialConfiguration Menu { get; } = new RadialConfiguration();

        public MenuPlacementConfiguration Placement { get; } = new MenuPlacementConfiguration();

        public string InitialProfile { get; private set; }

        public string CurrentProfile { get; set; }

        public List<ProfileConfiguration> Profiles { get; } = new List<ProfileConfiguration>();

        public List<CommandBinding> Bindings { get; } = new List<CommandBinding>();

        public List<MenuConfiguration> Menus { get; } = new List<MenuConfiguration>();

        //public string Stringify(bool comments)
        //{
        //    const string i1 = "  ";
        //    const string i2 = i1 + i1;
        //    const string i3 = i2 + i1;
        //    const string i4 = i3 + i1;

        //    var sb = new StringBuilder();
        //    if (comments)
        //    {
        //        sb.AppendLine("# FPS");
        //    }

        //    sb.AppendLine($"fps = {Fps}");

        //    if (comments)
        //    {
        //        sb.AppendLine("# Deadzones");
        //    }

        //    sb.AppendLine($"deadzone.ls = {100d * Deadzone.Ls}%");
        //    sb.AppendLine($"deadzone.rs = {100d * Deadzone.Rs}%");
        //    sb.AppendLine($"deadzone.lt = {100d * Deadzone.Lt}%");
        //    sb.AppendLine($"deadzone.rt = {100d * Deadzone.Rt}%");
        //    sb.AppendLine();

        //    if (comments)
        //    {
        //        sb.AppendLine("# Buttons");
        //    }

        //    sb.AppendLine($"hold.duration = {Hold.Duration}");
        //    sb.AppendLine($"repeat.delay = {Repeat.Delay}");
        //    sb.AppendLine($"repeat.interval = {Repeat.Interval}");
        //    sb.AppendLine();

        //    if (comments)
        //    {
        //        sb.AppendLine("# Mouse");
        //    }

        //    sb.AppendLine($"mouse.speed = {Mouse.Speed}");
        //    sb.AppendLine($"mouse.acceleration = {Mouse.Acceleration}");
        //    sb.AppendLine($"mouse.invertX = {Mouse.InvertX}");
        //    sb.AppendLine($"mouse.invertY = {Mouse.InvertY}");
        //    sb.AppendLine();

        //    if (comments)
        //    {
        //        sb.AppendLine("# Scrolling");
        //    }

        //    sb.AppendLine($"scroll.speed = {Scroll.Speed}");
        //    sb.AppendLine($"scroll.acceleration = {Scroll.Acceleration}");
        //    sb.AppendLine($"scroll.invertX = {Scroll.InvertX}");
        //    sb.AppendLine($"scroll.invertY = {Scroll.InvertY}");
        //    sb.AppendLine();

        //    if (comments)
        //    {
        //        sb.AppendLine("# Radial menu");
        //    }

        //    sb.AppendLine($"menu.minradius = {100d * Menu.MinRadius}%");
        //    sb.AppendLine($"menu.smoothing = {Menu.Smoothing}");
        //    sb.AppendLine($"menu.invertX = {Menu.InvertX}");
        //    sb.AppendLine($"menu.invertY = {Menu.InvertY}");
        //    sb.AppendLine($"menu.scale = {Placement.Scale}");
        //    sb.AppendLine($"menu.placement = {Placement.MenuPosition}");
        //    sb.AppendLine();

        //    if (CurrentProfile != null)
        //    {
        //        if (comments)
        //        {
        //            sb.AppendLine("# Initial profile");
        //        }

        //        sb.AppendLine($"profile.current = \"{Escape(InitialProfile)}\"");
        //        sb.AppendLine();
        //    }

        //    if (Profiles != null && Profiles.Any())
        //    {
        //        if (comments)
        //        {
        //            sb.AppendLine("# Profiles");
        //            sb.AppendLine();
        //        }

        //        foreach (var profile in Profiles)
        //        {
        //            sb.AppendLine($"profile \"{Escape(profile.Name)}\"");
        //            foreach (var binding in profile.Bindings)
        //            {
        //                sb.AppendLine(i1 + binding);
        //            }

        //            sb.AppendLine("end profile");
        //            sb.AppendLine();
        //        }
        //    }

        //    if (Bindings != null && Bindings.Any())
        //    {
        //        if (comments)
        //        {
        //            sb.AppendLine("# Global command bindings");
        //        }

        //        foreach (var binding in Bindings)
        //        {
        //            sb.AppendLine(binding.ToString());
        //        }

        //        sb.AppendLine();
        //    }

        //    if (Menus != null && Menus.Any())
        //    {
        //        if (comments)
        //        {
        //            sb.AppendLine("# Menus");
        //        }

        //        foreach (var menu in Menus)
        //        {
        //            sb.AppendLine($"menu \"{Escape(menu.Name)}\"");
        //            if (menu.Help != null)
        //            {
        //                sb.AppendLine(i1 + "help");
        //                if (menu.Help.HelpText != null)
        //                {
        //                    sb.AppendLine($"{i2}text = \"{Escape(menu.Help.HelpText)}\"");
        //                }

        //                foreach (var keyHelp in menu.Help.InputHelpTexts)
        //                {
        //                    sb.AppendLine($"{i2}{keyHelp.Key} = \"{Escape(keyHelp.Description)}\"");
        //                }

        //                sb.AppendLine(i1 + "end help");
        //            }

        //            if (menu.Pages != null)
        //            {
        //                foreach (var page in menu.Pages)
        //                {
        //                    sb.AppendLine(i1 + "page");
        //                    if (page.Help != null)
        //                    {
        //                        sb.AppendLine(i2 + "help");
        //                        if (page.Help.HelpText != null)
        //                        {
        //                            sb.AppendLine($"{i3}text = \"{Escape(page.Help.HelpText)}\"");
        //                        }

        //                        foreach (var keyHelp in page.Help.InputHelpTexts)
        //                        {
        //                            sb.AppendLine($"{i3}{keyHelp.Key} = \"{Escape(keyHelp.Description)}\"");
        //                        }

        //                        sb.AppendLine(i2 + "end help");
        //                    }

        //                    if (page.Items != null)
        //                    {
        //                        foreach (var item in page.Items)
        //                        {
        //                            sb.AppendLine(i2 + "item");

        //                            if (item.Help != null)
        //                            {
        //                                sb.AppendLine(i3 + "help");
        //                                if (item.Help.HelpText != null)
        //                                {
        //                                    sb.AppendLine($"{i4}text = \"{Escape(item.Help.HelpText)}\"");
        //                                }

        //                                foreach (var keyHelp in item.Help.InputHelpTexts)
        //                                {
        //                                    sb.AppendLine($"{i4}{keyHelp.Key} = \"{Escape(keyHelp.Description)}\"");
        //                                }

        //                                sb.AppendLine(i3 + "end help");
        //                            }

        //                            if (item.Icon != null)
        //                            {
        //                                sb.AppendLine(
        //                                    $"{i3}icon = {string.Join(" ", item.Icon.Select(t => t.AsToken))}");
        //                            }

        //                            if (item.Name != null)
        //                            {
        //                                sb.AppendLine($"{i3}name = \"{Escape(item.Name)}\"");
        //                            }

        //                            if (item.Text != null)
        //                            {
        //                                sb.AppendLine($"{i3}text = \"{Escape(item.Text)}\"");
        //                            }

        //                            foreach (var binding in item.CommandBindings ?? new CommandBinding[0])
        //                            {
        //                                sb.AppendLine($"{i3}{binding}");
        //                            }

        //                            sb.AppendLine(i2 + "end item");
        //                        }
        //                    }

        //                    sb.AppendLine(i1 + "end page");
        //                }
        //            }

        //            sb.AppendLine("end menu");
        //            sb.AppendLine();
        //        }
        //    }

        //    return sb.ToString();
        //}

        private List<ProfileConfiguration> ProfilesWithoutMenu()
        {
            return Profiles.Where(p => !string.Equals(p.Name, "menu", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private string GetPreviousProfile(string currentProfile)
        {
            var profiles = ProfilesWithoutMenu();
            var index = profiles.FindIndex(p => string.Equals(p.Name, currentProfile, StringComparison.OrdinalIgnoreCase));
            return index == -1 ? "Default" : profiles[(index + profiles.Count - 1) % profiles.Count].Name;
        }

        private string GetNextProfile(string currentProfile)
        {
            var profiles = ProfilesWithoutMenu();
            var index = profiles.FindIndex(p => string.Equals(p.Name, currentProfile, StringComparison.OrdinalIgnoreCase));
            return index == -1 ? "Default" : profiles[(index + 1) % profiles.Count].Name;
        }

        private void Add(IConfigDescriptor descriptor)
        {
            descriptors[descriptor.Key] = descriptor;
        }
    }
}
