using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GamepadMapper.Configuration;
using GamepadMapper.Input;
using GamepadMapper.Menus;
using XInputDotNetPure;
using ButtonState = GamepadMapper.Input.ButtonState;

namespace GamepadMapper.Infrastructure
{
    public class ApplicationLoop
    {
        public ApplicationLoop(RootConfiguration configuration, ProfileCollection profileCollection, IMenuController menuController, IGamePadStateReader stateReader)
        {
            Configuration = configuration;
            ProfileCollection = profileCollection;
            MenuController = menuController;
            StateReader = stateReader;
        }

        public RootConfiguration Configuration { get; }

        public ProfileCollection ProfileCollection { get; }

        public IMenuController MenuController { get; }

        public IGamePadStateReader StateReader { get; }

        public async Task Run(CancellationToken cancellation)
        {
            var connected = false;
            var isEnabled = true;
            var lastBack = false;
            var lastStart = false;
            var lastFrame = DateTime.Now;
            var dirtyButtons = new HashSet<Button>();
            Profile profile = null;

            await Task.Delay(TimeSpan.FromMilliseconds(1000d / Configuration.Fps), cancellation);
            while (!cancellation.IsCancellationRequested)
            {
                var frameStart = DateTime.Now;
                double fps;

                if (!TryGetLowestIndex(out var state, out var playerIndex))
                {
                    if (connected)
                    {
                        // If we were connected, clear state to prevent stuck keys.
                        profile?.ClearState();
                        dirtyButtons.Clear();
                        MenuController.FlashMessage(null, "Connection lost", null);
                    }

                    connected = false;
                    fps = 5d;
                }
                else
                {
                    if (!connected)
                    {
                        MenuController.FlashMessage($"Profile: {Configuration.CurrentProfile ?? "None"}", "Connected", null);
                    }

                    connected = true;
                    fps = Configuration.Fps;
                }

                var inputState = InputState.FromGamePadState(state, Configuration.Deadzone);
                var frame = new FrameDetails(playerIndex, profile, frameStart, (frameStart - lastFrame).TotalMilliseconds, fps, inputState, connected);
                if (connected)
                {
                    if (dirtyButtons.Count != 0)
                    {
                        // Clear dirty buttons.
                        foreach (var pair in inputState.ButtonStates)
                        {
                            if (pair.Value.IsPressed)
                            {
                                continue;
                            }

                            if (dirtyButtons.Remove(pair.Key) && dirtyButtons.Count == 0)
                            {
                                break;
                            }
                        }
                    }

                    // Profile switching
                    var newProfile = GetCurrentProfile();
                    if (!ReferenceEquals(profile, newProfile))
                    {
                        profile?.ClearState();
                        foreach (var pair in inputState.ButtonStates)
                        {
                            if (pair.Value.IsPressed)
                            {
                                dirtyButtons.Add(pair.Key);
                            }
                        }

                        profile = newProfile;
                    }

                    // Input disabling
                    var back = state.Buttons.Back == XInputDotNetPure.ButtonState.Pressed;
                    var start = state.Buttons.Start == XInputDotNetPure.ButtonState.Pressed;
                    if (back && start)
                    {
                        if (!lastBack || !lastStart)
                        {
                            isEnabled = !isEnabled;
                            dirtyButtons.Add(Button.Back);
                            dirtyButtons.Add(Button.Start);
                            profile?.ClearState();
                        }
                    }

                    lastBack = back;
                    lastStart = start;
                    if (profile != null && isEnabled)
                    {
                        UpdateProfile(profile, inputState, frame, dirtyButtons);
                    }
                }

                MenuController.Update(frame);

                lastFrame = frameStart;

                // Try to keep a constant frame rate.
                await Task.Delay(TimeSpan.FromMilliseconds(
                    Math.Max(1000d / fps - (DateTime.Now - frameStart).TotalMilliseconds, 1d)),
                    cancellation);
            }
        }

        private Profile GetCurrentProfile()
        {
            if (MenuController.IsOpen && ProfileCollection.MenuProfile != null)
            {
                return ProfileCollection.MenuProfile;
            }

            var currentKey = Configuration.CurrentProfile;
            if (currentKey != null && ProfileCollection.Profiles.TryGetValue(currentKey, out var profile))
            {
                return profile;
            }

            if (ProfileCollection.Profiles.TryGetValue("default", out profile))
            {
                return profile;
            }

            return ProfileCollection.Profiles.FirstOrDefault().Value;
        }

        private bool TryGetLowestIndex(out GamePadState gamePadState, out PlayerIndex playerIndex)
        {
            gamePadState = StateReader.GetState(PlayerIndex.One);
            playerIndex = PlayerIndex.One;
            if (gamePadState.IsConnected)
            {
                return true;
            }

            gamePadState = StateReader.GetState(PlayerIndex.Two);
            playerIndex = PlayerIndex.Two;
            if (gamePadState.IsConnected)
            {
                return true;
            }

            gamePadState = StateReader.GetState(PlayerIndex.Three);
            playerIndex = PlayerIndex.Three;
            if (gamePadState.IsConnected)
            {
                return true;
            }

            gamePadState = StateReader.GetState(PlayerIndex.Four);
            playerIndex = PlayerIndex.Four;
            if (gamePadState.IsConnected)
            {
                return true;
            }

            return false;
        }

        private static void UpdateProfile(Profile profile, InputState inputState, FrameDetails frame, HashSet<Button> dirtyButtons)
        {
            const int modA = (int)InputKey.ModA;
            var isModifierDown = false;
            foreach (var modifierKey in profile.Modifiers)
            {
                if (inputState.ButtonStates[modifierKey].IsPressed)
                {
                    isModifierDown = true;
                    break;
                }
            }

            profile.LeftAnalogHandler?.Update(inputState.LeftAnalogState, ThumbStick.Left, frame);
            profile.RightAnalogHandler?.Update(inputState.RightAnalogState, ThumbStick.Right, frame);
            var nonPressedKey = new ButtonState();
            // Some enum hacking is done below...
            // Button is converted to InputKey 1 to 1, and for converting to Mod<Button> it's added by an offset.
            foreach (var pair in inputState.ButtonStates)
            {
                if (profile.Modifiers.Contains(pair.Key))
                {
                    // Modifiers are ignored.
                    continue;
                }

                if (dirtyButtons.Contains(pair.Key))
                {
                    continue;
                }

                var button = (InputKey)pair.Key;
                var modButton = (InputKey)(modA + (int)pair.Key);
                if (profile.ButtonHandlers.TryGetValue(button, out var buttonHandler))
                {
                    buttonHandler?.Update(isModifierDown ? nonPressedKey : pair.Value, button, frame);
                }

                if (profile.ButtonHandlers.TryGetValue(modButton, out var modButtonHandler))
                {
                    modButtonHandler?.Update(isModifierDown ? pair.Value : nonPressedKey, modButton, frame);
                }
            }
        }
    }
}
