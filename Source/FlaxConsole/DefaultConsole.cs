using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxConsole;

/// <summary>
/// DefaultConsole Script.
/// </summary>
public class DefaultConsole : Console {
    [ShowInEditor]
    [Serialize]
    private InputEvent SwitchEvent;

    [ShowInEditor]
    [Serialize]
    private InputEvent ExecuteEvent;

    [ShowInEditor]
    [Serialize]
    private InputEvent GetPreviousEvent;

    [ShowInEditor]
    [Serialize]
    private InputEvent GetNextEvent;

    [ShowInEditor]
    [Serialize]
    private ControlReference<TextBox> textBox;

    [ShowInEditor]
    [Serialize]
    private ControlReference<TextBox> text;

    private Actor child;

    public override void OnStart() {
        child = Actor.GetChild(0);

        var settings = PluginManager.GetPlugin<FlaxConsole>().Settings;

        SwitchEvent = settings.SwitchEvent;
        ExecuteEvent = settings.ExecuteEvent;
        GetPreviousEvent = settings.GetPreviousEvent;
        GetNextEvent = settings.GetNextEvent;

        base.OnStart();
    }

    public override void OnEnable() {
        SwitchEvent.Pressed += SwitchEvent_Pressed;
        ExecuteEvent.Pressed += ExecuteEvent_Pressed;
        GetNextEvent.Pressed += GetNextEvent_Pressed;
        GetPreviousEvent.Pressed += GetPreviousEvent_Pressed;
    }


    public override void OnDisable() {
        SwitchEvent.Pressed -= SwitchEvent_Pressed;
        ExecuteEvent.Pressed -= ExecuteEvent_Pressed;
        GetNextEvent.Pressed -= GetNextEvent_Pressed;
        GetPreviousEvent.Pressed -= GetPreviousEvent_Pressed;
    }

    private void GetPreviousEvent_Pressed() {
        if (IsOpen 
            && GetPreviousCommand() is string valid) {
            textBox.Control.SetText(valid);
        }
    }

    private void GetNextEvent_Pressed() {
        if (IsOpen
            && GetNextCommand() is string valid) {
            textBox.Control.SetText(valid);
        }
    }

    private void ExecuteEvent_Pressed() {
        Execute(textBox.Control.Text);

        textBox.Control.SetText("");

        textBox.Control.Focus();
    }

    private void SwitchEvent_Pressed() {
        Switch();
    }

    protected override void Close() {
        bool refocus = Engine.HasGameViewportFocus;

        textBox.Control.SetText("");
        child.IsActive = false;

        if (refocus) Engine.FocusGameViewport();

        Time.TimeScale = 1;
    }

    protected override void Open() {
        child.IsActive = true;
        textBox.Control.Focus();

        Time.TimeScale = 0;

        Screen.CursorLock = CursorLockMode.Clipped;
        Screen.CursorVisible = true;
    }

    protected override void Log(string message) {
        text.Control.Text += $"\n{message}";
    }
}
