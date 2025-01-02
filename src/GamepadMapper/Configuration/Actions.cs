using System.Collections.Generic;
using System.Linq;
using WindowsInput.Native;

namespace GamepadMapper.Configuration
{
    public abstract class ActionDescriptor
    {
        public override string ToString() => Stringify();

        public abstract string Stringify();
    }

    public class CommandAction : ActionDescriptor
    {
        public CommandAction(string command)
        {
            Command = command;
        }

        public string Command { get; }

        public override string Stringify() => $"command({Utils.Escape(Command)})";
    }

    public class KeyAction : ActionDescriptor
    {
        public KeyAction(IEnumerable<VirtualKeyCode> keys)
        {
            Keys = keys?.ToArray() ?? new VirtualKeyCode[0];
        }

        public IEnumerable<VirtualKeyCode> Keys { get; }

        public override string Stringify() => string.Join("+", Keys);
    }

    public class Macro : ActionDescriptor
    {
        public Macro(IEnumerable<ActionDescriptor> actions)
        {
            Actions = actions;
        }

        public IEnumerable<ActionDescriptor> Actions { get; }

        public override string Stringify() => string.Join(";", Actions);
    }

    public class ShowMenuAction : ActionDescriptor
    {
        public ShowMenuAction(string menu)
        {
            Menu = menu;
        }

        public string Menu { get; }

        public override string Stringify() => $"show({Utils.Escape(Menu)})";
    }

    public class RunProgramAction : ActionDescriptor
    {
        public RunProgramAction(string path, string arguments)
        {
            Path = path;
            Arguments = arguments;
        }

        public string Path { get; }

        public string Arguments { get; }

        public override string Stringify() =>
            $"run({Utils.Escape(Path)}" + (Arguments != null ? $",{Utils.Escape(Arguments)})" : ")");
    }

    public class FlashConfigurationAction : ActionDescriptor
    {
        public FlashConfigurationAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Stringify() => $"flashcfg({Utils.Escape(Key)})";
    }

    public class FlashMessageAction : ActionDescriptor
    {
        public FlashMessageAction(string title, string message, string modifier)
        {
            Title = title;
            Message = message;
            Modifier = modifier;
        }

        public string Title { get; }

        public string Message { get; }

        public string Modifier { get; }

        public override string Stringify() => $"flashmsg({Utils.Escape(Title)},{Utils.Escape(Message)},{Utils.Escape(Modifier)})";
    }

    public class NoOpAction : ActionDescriptor
    {
        public override string Stringify() => "nothing";
    }

    public class IncrementConfigurationAction : ActionDescriptor
    {
        public IncrementConfigurationAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Stringify() => $"increment({Utils.Escape(Key)})";
    }

    public class DecrementConfigurationAction : ActionDescriptor
    {
        public DecrementConfigurationAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Stringify() => $"decrement({Utils.Escape(Key)})";
    }

    public class ToggleConfigurationAction : ActionDescriptor
    {
        public ToggleConfigurationAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Stringify() => $"toggle({Utils.Escape(Key)})";
    }

    public class ResetConfigurationAction : ActionDescriptor
    {
        public ResetConfigurationAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public override string Stringify() => $"reset({Utils.Escape(Key)})";
    }

    public class SetConfigurationAction : ActionDescriptor
    {
        public SetConfigurationAction(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string Stringify() => $"set({Utils.Escape(Key)}, {Utils.Escape(Value)})";
    }

    public class SendCharacterAction : ActionDescriptor
    {
        public SendCharacterAction(char c)
        {
            Character = c;
        }

        public char Character { get; }

        public override string Stringify() => $"sendchar({Utils.Escape(Character.ToString())})";
    }

    public class SendStringAction : ActionDescriptor
    {
        public SendStringAction(string str)
        {
            String = str;
        }

        public string String { get; }

        public override string Stringify() => $"sendstr({Utils.Escape(String)})";
    }

    public class SetPageAction : ActionDescriptor
    {
        public SetPageAction(int page)
        {
            Page = page;
        }

        public int Page { get; }

        public override string Stringify() => $"setpage({Page})";
    }
}
