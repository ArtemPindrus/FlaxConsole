using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxConsole;

/// <summary>
/// ConsoleEnvironmentSettings class.
/// </summary>
public class ConsoleEnvironmentSettings
{
    /// <summary>
    /// Whether to ship console to the cooked game.
    /// </summary>
    public bool ShipConsole;


    public LayersMask DestroyMask;
    public SceneReference ConsoleScene;
    public int HistoryCapacity;

    [Header("Input")]
    public InputEvent SwitchEvent = new();
    public InputEvent ExecuteEvent = new();
    public InputEvent GetPreviousEvent = new();
    public InputEvent GetNextEvent = new();
}
