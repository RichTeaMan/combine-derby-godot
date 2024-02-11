using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;

public partial class RenderStudio : Node3D
{
    readonly static Color MAGIC_PINK = new(1.0f, 0.0f, 1.0f);

    readonly static Color TRANSPARENT = new(0.0f, 0.0f, 0.0f, 0.0f);

    private bool rotating = false;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;
    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;
    private Node3D Container => GetNode<Node3D>("%container");
    private Camera3D Camera => GetNode<Camera3D>("%camera");
    private Label StatusLabel => GetNode<Label>("%label_status");

    private int framesToScreenshot = -1;

    private Node[] previousHiddenNodes = Array.Empty<Node>();

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
        if (framesToScreenshot > 0)
        {
            framesToScreenshot--;
            return;
        }
        if (framesToScreenshot == 0)
        {
            framesToScreenshot = -1;

            string filepath = $"screenshot-{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)}.png";
            using var screenShot = GetViewport().GetTexture().GetImage();
            using var transparentScreenshot = Image.Create(screenShot.GetWidth(), screenShot.GetHeight(), false, Image.Format.Rgba8);
            foreach (var x in Enumerable.Range(0, screenShot.GetWidth()))
            {
                foreach (var y in Enumerable.Range(0, screenShot.GetHeight()))
                {
                    var pixel = screenShot.GetPixel(x, y);
                    if (pixel == MAGIC_PINK)
                    {
                        transparentScreenshot.SetPixel(x, y, TRANSPARENT);
                    }
                    else
                    {
                        transparentScreenshot.SetPixel(x, y, pixel);
                    }
                }
            }
            transparentScreenshot.SavePng(filepath);
            
            StatusLabel.Text = $"Screenshot saved to {filepath}.";
            GD.Print($"Screenshot saved to {filepath}.");

            previousHiddenNodes.RestoreVisibleNodes();
        }

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
        if (framesToScreenshot > 0)
        {
            return;
        }
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
            previousHiddenNodes = GetTree().Root.HideVisibleNodeChildrenRecursive();
            Container.Visible = true;
            Container.ChildrenRecursive().ToArray().RestoreVisibleNodes();
            List<Node> parents = new List<Node>();
            var parent = Container.GetParent();
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.GetParent();
            }
            parents.ToArray().RestoreVisibleNodes();

            // it will take at least a frame for visibilty changes to render, so set a timer to do it later.
            framesToScreenshot = 1;
        }

    }
}
