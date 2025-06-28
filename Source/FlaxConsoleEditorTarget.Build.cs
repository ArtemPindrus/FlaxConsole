using Flax.Build;

public class FlaxConsoleEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("FlaxConsole");
        Modules.Add("FlaxConsoleEditor");
    }
}
