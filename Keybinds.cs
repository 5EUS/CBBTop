using System;
using System.Collections.Generic;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;

namespace CBBTop;

public class Keybinds(List<KeyBinding> keyBindings)
{
    public List<KeyBinding> KeyBindings { get; } = keyBindings;

    public void AddKeyBinding(Key key, Action action, KeyModifiers? modifiers = null)
    {
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(key, modifiers ?? KeyModifiers.None),
            Command = new RelayCommand(action)
        });
    }
}