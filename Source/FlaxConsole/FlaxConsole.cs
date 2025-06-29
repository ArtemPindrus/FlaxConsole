using System;
using FlaxEngine;

namespace FlaxConsole
{
    /// <summary>
    /// The sample game plugin.
    /// </summary>
    /// <seealso cref="FlaxEngine.GamePlugin" />
    public class FlaxConsole : GamePlugin
    {
        public ConsoleEnvironmentSettings Settings { get; private set; }
        public Console Console { get; private set; }

        /// <inheritdoc />
        public FlaxConsole()
        {
            _description = new PluginDescription
            {
                Name = "FlaxConsole",
                Category = "Other",
                Author = "ArtemPindrus",
                AuthorUrl = null,
                HomepageUrl = null,
                RepositoryUrl = "https://github.com/ArtemPindrus/FlaxConsole",
                Description = "In-game console.",
                Version = new Version(),
                IsAlpha = false,
                IsBeta = false,
            };
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            var settingsAsset = Engine.GetCustomSettings("ConsoleEnvironmentSettings");

            if (settingsAsset == null) {
                Debug.LogError("Couldn't find ConsoleEnvironmentSettings custom settings.");
            }

            Settings = (ConsoleEnvironmentSettings)settingsAsset.Instance;

            if (!Engine.IsEditor
                && !Settings.ShipConsole) return;

            Level.LoadScene(Settings.ConsoleScene);

            Console = Level.FindScript<Console>();
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            // Use it to cleanup data
            base.Deinitialize();
        }
    }
}
