using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ProgStart : Node
{
    const string RENDER_ARG = "render";

    const string EDITOR_ARG = "vehicle-editor";

#if NO_TOOLS
    const bool ENABLE_TOOLS = false;
#else
    const bool ENABLE_TOOLS = true;
#endif

    public override void _Ready()
    {

        GD.Print($"cmd: {OS.GetCmdlineArgs().Join(" ")}");

        var arguments = new Dictionary<string, string>();
        // Parse valid command-line arguments into a dictionary
        foreach (var argument in OS.GetCmdlineArgs().Where(a => a.StartsWith("--")))
        {
            var splits = argument.Split("=");
            string key = splits[0].TrimStart('-');
            string value = "true";
            if (splits.Length > 1)
            {
                value = splits[1];
            }
            arguments.Add(key, value);
        }

        // check render arg
        if (ENABLE_TOOLS && arguments.TryGetValue(RENDER_ARG, out string renderArg))
        {
            var renderStudioScene = GD.Load<PackedScene>($"res://tools/render_studio.tscn");
            var renderStudio = renderStudioScene.Instantiate<RenderStudio>();
            AddChild(renderStudio);
            renderStudio.LoadFromResPath(renderArg);
        }

        // check editor arg
        else if (arguments.TryGetValue(EDITOR_ARG, out string _editorArg))
        {
            var scene = GD.Load<PackedScene>("res://editor/editor.tscn");
            if (scene != null)
            {
                var editor = scene.Instantiate();
                AddChild(editor);
            }
            else
            {
                GD.PrintErr("Failed to load editor.");
                GetTree().Quit(1);
            }
        }

        // no args, load start screen
        else
        {
            var scene = GD.Load<PackedScene>("res://ui/start_screen.tscn");
            if (scene != null)
            {
                var start = scene.Instantiate();
                AddChild(start);
            }
            else
            {
                GD.PrintErr("Failed to load start.");
                GetTree().Quit(1);
            }
        }
    }

}
