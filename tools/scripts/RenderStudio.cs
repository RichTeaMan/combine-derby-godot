using System;
using System.Globalization;
using Godot;

public partial class RenderStudio : Node3D
{
    private bool rotating = false;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;
    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;
    private Node3D Container => GetNode<Node3D>("%container");

    private Node3D Camera => GetNode<Camera3D>("%camera");

    public void LoadFromResPath(string resource)
    {

        var scene = GD.Load<PackedScene>($"res://{(resource ?? "").Replace("res://", "")}");
        if (scene == null)
        {
            GD.PrintErr("Failed to load render scene. --render= must point to a valid scene file.");
            GetTree().Quit(1);
        }

        Container.AddChild(scene.Instantiate());
    }

    public override void _Process(double delta)
    {
        if (rotating)
        {
            nextMousePosition = GetViewport().GetMousePosition();
            Container.RotateY((nextMousePosition.X - prevMousePosition.X) * rotation_constant * (float)delta);
            Container.RotateX((nextMousePosition.Y - prevMousePosition.Y) * rotation_constant * (float)delta);
            prevMousePosition = nextMousePosition;
        }

        if (Input.IsActionJustPressed("zoom_in"))
        {
            Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y, Camera.Position.Z - zoom_constant);
        }

        if (Input.IsActionJustPressed("zoom_out"))
        {
            Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y, Camera.Position.Z + zoom_constant);
        }
    }

    public override void _UnhandledInput(InputEvent _inputEvent)
    {
        if (Input.IsActionJustPressed("rotate"))
        {
            rotating = true;
            prevMousePosition = GetViewport().GetMousePosition();
        }
        if (Input.IsActionJustReleased("rotate"))
        {
            rotating = false;
        }
        if (Input.IsActionJustPressed("screenshot"))
        {
            string filepath = $"screenshot-{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)}.png";
            var screenShot = GetViewport().GetTexture().GetImage();
            screenShot.SavePng(filepath);
            GD.Print($"Screenshot save to {filepath}.");
        }

    }
}
