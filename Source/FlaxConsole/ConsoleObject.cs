using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlaxEngine;
using FlaxEngine.Utilities;
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

    private HashSet<PropertyInfo> properties = new();

    public override void OnAwake() {
        Console c = PluginManager.GetPlugin<FlaxConsole>().Console;

        LuaTable actorTable = new LuaTable() {
            Metatable = new()
        };

        AddMembers(Actor, actorTable);

        foreach (var script in Actor.Scripts) {
            var subTable = new LuaTable() {
                Metatable = new()
            };
            actorTable[script.GetType().Name] = subTable;

            AddMembers(script, subTable);
        }
        
        c.AddToEnvironment(!string.IsNullOrWhiteSpace(CustomName) ? CustomName : Actor.Name, actorTable);
    }

    private void AddMembers(object instance, LuaTable table) {
        var type = instance.GetType();

        if (type == typeof(ConsoleObject)) return;

        var metatable = table.Metatable;

        var methods = type.GetMethods(BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic).Where(x => x.GetParameters().Length == 0);

        foreach (var m in methods) {
            table[m.Name] = new LuaFunction((ctx, bf, ct) => {
                m.Invoke(instance, null);

                return new(0);
            });
        }

        var properties = type.GetProperties(BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic);

        this.properties.AddRange(properties);

        metatable["__newindex"] = new LuaFunction((ctx, bf, ct) => {
            string key = ctx.GetArgument<string>(1);

            var propertyInfo = properties.FirstOrDefault(x => x.Name == key);

            if (propertyInfo == null) return new(0);

            var value = ctx.GetArgument<object>(2);

            propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));

            return new(0);
        });

        metatable["__index"] = new LuaFunction((ctx, bf, ct) => {
            string key = ctx.GetArgument<string>(1);

            var propertyInfo = properties.FirstOrDefault(x => x.Name == key);

            if (propertyInfo == null) return new(0);

            bf.Span[0] = propertyInfo.GetValue(instance).ToString();

            return new(1);
        });
    }
}
