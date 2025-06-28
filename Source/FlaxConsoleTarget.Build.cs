using Flax.Build;

public class FlaxConsoleTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("FlaxConsole");
    }
}
