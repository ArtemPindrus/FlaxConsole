using Flax.Build;
using Flax.Build.NativeCpp;
using System.IO;

public class FlaxConsole : GameModule
{
    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        // Here you can modify the build options for your game module
        // To reference another module use: options.PublicDependencies.Add("Audio");
        // To add C++ define use: options.PublicDefinitions.Add("COMPILE_WITH_FLAX");
        // To learn more see scripting documentation.
        BuildNativeCode = false;

        options.ScriptingAPI.FileReferences.Add(Path.Combine(FolderPath, "..", "Lua.dll"));
    }
}
