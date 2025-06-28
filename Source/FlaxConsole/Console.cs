using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlaxEditor.Content.Settings;
using FlaxEngine;
using Lua;

namespace FlaxConsole;

/// <summary>
/// Console Script.
/// </summary>
public abstract class Console : Script
{
    private ConsoleEnvironmentSettings settings;

    [ShowInEditor]
    [ReadOnly]
    private Stack<string> previous;

    [ShowInEditor]
    [ReadOnly]
    private Stack<string> next;

    [ShowInEditor]
    [ReadOnly]
    public bool IsOpen { get; private set; }

    [Serialize]
    [ShowInEditor]
    protected bool startState;


    private LuaState lua;

    public override void OnStart() {
        settings = PluginManager.GetPlugin<FlaxConsole>().Settings;
        previous = new(settings.HistoryCapacity);
        next = new(settings.HistoryCapacity);

        EnsureState(startState);

        lua = LuaState.Create();

        CreateDefaultEnvironment();
    }

    public string? GetPreviousCommand() {
        if (previous.Count > 0) {
            Debug.Log("Prev");

            string p = previous.Pop();

            next.Push(p);
            return p;
        } else return null;
    }

    public string? GetNextCommand() {
        if (next.Count > 0) {
            Debug.Log("Next");

            string n = next.Pop();

            previous.Push(n);
            return n;
        } else return null;
    }

    public void EnsureState(bool open) {
        if (open) {
            Open();
            IsOpen = true;
        } else {
            Close();
            IsOpen = false;
        }
    }

    [Button]
    public void Switch() {
        if (IsOpen) Close();
        else Open();

        IsOpen = !IsOpen;
    }

    public async ValueTask Execute(string str) {
        try {
            await lua.DoStringAsync(str);
        } catch(LuaException l) {
            Log(l.Message);
        } finally {
            while(next.Count > 0) {
                previous.Push(next.Pop());
            }

            previous.Push(str);
        }
    }

    public void AddToEnvironment(string key, LuaValue luaValue) {
        lua.Environment[key] = luaValue;
    }

    protected abstract void Log(string message);

    protected abstract void Close();

    protected abstract void Open();

    private void CreateDefaultEnvironment() {
        lua.Environment["destroyC"] = new LuaFunction((context, buffer, ct) => {
            Camera mainCamera = Camera.MainCamera;

            if (Physics.RayCast(mainCamera.Position, mainCamera.Transform.Forward, out var hit,
                layerMask: settings.DestroyMask,
                hitTriggers: false)) {
                if (hit.Collider.Parent != null) {
                    Destroy(hit.Collider.Parent);
                    Log($"Destroyed {hit.Collider.Parent.Name}");
                } else {
                    Destroy(hit.Collider);
                    Log($"Destroyed {hit.Collider.Name}");
                }
            } else Log("Failed to find object from camera Raycast.");

            return new(0);
        });

        lua.Environment["log"] = new LuaFunction((context, buffer, ct) => {
            Log(context.GetArgument(0).ToString());
            
            return new(0);
        });
    }
}
