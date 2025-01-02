using System.Collections.Generic;
using WindowsInput;
using GamepadMapper.Actuators;
using GamepadMapper.Configuration;
using GamepadMapper.Handlers;
using GamepadMapper.Input;
using GamepadMapper.Menus;

namespace GamepadMapper.Infrastructure
{
    public interface IProfileFactory
    {
        Profile CreateProfile(ProfileConfiguration profileConfig);
    }

    public class ProfileFactory : IProfileFactory
    {
        public ProfileFactory(
            RootConfiguration rootConfiguration,
            FlagCollection flags,
            IActionFactory actionFactory,
            IKeyboardSimulator keyboard,
            IMouseSimulator mouse,
            IMenuController menuController)
        {
            RootConfiguration = rootConfiguration;
            Flags = flags;
            ActionFactory = actionFactory;
            Keyboard = keyboard;
            Mouse = mouse;
            MenuController = menuController;
        }

        public RootConfiguration RootConfiguration { get; }

        public FlagCollection Flags { get; }

        public IActionFactory ActionFactory { get; }

        public IKeyboardSimulator Keyboard { get; }

        public IMouseSimulator Mouse { get; }

        public IMenuController MenuController { get; }

        public Profile CreateProfile(ProfileConfiguration profileConfig)
        {
            IAnalogHandler leftHandler = null;
            IAnalogHandler rightHandler = null;
            var handlers = new Dictionary<InputKey, IButtonHandler>();
            var modifiers = new HashSet<Button>();
            void SetAnalog(AnalogBinding analogBinding, IAnalogHandler analogHandler)
            {
                if (analogBinding.ThumbStick == ThumbStick.Left)
                {
                    leftHandler = analogHandler;
                }
                else
                {
                    rightHandler = analogHandler;
                }
            }

            foreach (var binding in profileConfig.Bindings)
            {
                switch (binding)
                {
                    case MouseMapping mouseMapping:
                        SetAnalog(mouseMapping, new MovementHandler(new MouseMovementActuator(Mouse), RootConfiguration.Mouse));
                        break;
                    case ScrollMapping scrollMapping:
                        SetAnalog(scrollMapping, new MovementHandler(new MouseScrollActuator(Mouse), RootConfiguration.Scroll));
                        break;
                    case RadialMenuMapping radialMenuMapping:
                        SetAnalog(radialMenuMapping, new RadialHandler(new MenuPointerActuator(MenuController), RootConfiguration.Menu));
                        break;
                    case KeyMapping keyMapping:
                        handlers[keyMapping.InputKey] = new KeyMapHandler(new KeyMapActuator(Keyboard, Mouse, keyMapping.Keys));
                        break;
                    case ModMapping modMapping:
                        if (modMapping.InputKey < InputKey.ModA)
                        {
                            modifiers.Add((Button)modMapping.InputKey);
                        }

                        break;
                    case FlagMapping flagMapping:
                        handlers[flagMapping.InputKey] = new KeyMapHandler(new FlagMapActuator(Flags, flagMapping.Flag));
                        break;
                    case PressBinding pressBinding:
                        handlers[pressBinding.InputKey] = pressBinding.Repeat
                            ? (IButtonHandler)new KeyPressRepeatHandler(ActionFactory.Create(pressBinding.Action), RootConfiguration.Repeat)
                            : new KeyPressHandler(ActionFactory.Create(pressBinding.Action));
                        break;
                    case PressHoldBinding pressHoldBinding:
                        handlers[pressHoldBinding.InputKey] = new KeyPressHoldHandler(
                            ActionFactory.Create(pressHoldBinding.PressAction),
                            ActionFactory.Create(pressHoldBinding.HoldAction),
                            RootConfiguration.Hold);
                        break;
                }
            }

            return new Profile(profileConfig.Name, modifiers, leftHandler, rightHandler, handlers);
        }
    }
}