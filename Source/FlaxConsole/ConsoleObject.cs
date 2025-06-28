using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine;
using Lua;

namespace FlaxConsole;

/// <summary>
/// ConsoleObject Script.
/// </summary>
public class ConsoleObject : Script
{
    [Serialize]
    [ShowInEditor]
    public string? CustomName { get; private set; }

    public override void OnAwake() {
        Console c = PluginManager.GetPlugin<FlaxConsole>().Console;

        LuaTable actorTable = new LuaTable() {
            Metatable = new()
        };

        Add(Actor, actorTable);

        foreach (var script in Actor.Scripts) {
            Add(script, actorTable);
        }
        
        c.AddToEnvironment(!string.IsNullOrWhiteSpace(CustomName) ? CustomName : Actor.Name, actorTable);
    }

    private void Add(object instance, LuaTable actorTable) {
        var type = instance.GetType();

        if (type == typeof(ConsoleObject)) return;

        var metaTable = actorTable.Metatable;

        Debug.Log(type.FullName);

        var methods = type.GetMethods(System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic).Where(x => x.GetParameters().Length == 0);

        foreach (var m in methods) {
            actorTable[m.Name] = new LuaFunction((ctx, bf, ct) => {
                m.Invoke(instance, null);

                return new(0);
            });
        }

        var properties = type.GetProperties(System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic);

        metaTable["__newindex"] = new LuaFunction((ctx, bf, ct) => {
            string key = ctx.GetArgument<string>(1);

            Debug.Log(key);

            var propertyInfo = properties.FirstOrDefault(x => x.Name == key);

            Debug.Log(propertyInfo != null);

            if (propertyInfo == null) return new(0);

            var value = ctx.GetArgument<object>(2);

            propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));

            return new(0);
        });
    }
}
