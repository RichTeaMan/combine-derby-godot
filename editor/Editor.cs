using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Editor : Node3D
{

    private bool rotating = false;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;
    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;

    const string SELECTED_GRP = "selected";

    private List<VehiclePart> parts = new();

    private List<VehiclePart> selectedParts = new();

    private CdVehicle vehicle;

    private Shader highlightedShader;

    private Node3D Container => GetNode<Node3D>("%container");

    private VBoxContainer PartsContainer => GetNode<VBoxContainer>("%parts_container");

    private Node3D Gimbal => GetNode<Node3D>("%gimbal");

    private Node3D Camera => GetNode<Node3D>("%camera");

    private ColorPicker ColourPicker => GetNode<ColorPicker>("%ColorPicker");

    public override void _Ready()
    {
        base._Ready();
        highlightedShader = GD.Load<Shader>("res://shaders/highlighted.gdshader");

        vehicle = new CdVehicle
        {
            Freeze = true
        };

        Container.AddChild(vehicle);

        GD.Print("Getting parts...");
        var parts = VehiclePart.PartsInit();
        foreach (var part in parts)
        {
            var partButton = new Button
            {
                Text = part.Name
            };
            partButton.Pressed += () => { partButtonPressed(part); };
            PartsContainer.AddChild(partButton);
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
    }

    public override void _Process(double delta)
    {
        if (rotating)
        {
            nextMousePosition = GetViewport().GetMousePosition();
            Gimbal.RotateY((nextMousePosition.X - prevMousePosition.X) * rotation_constant * (float)delta);
            Gimbal.RotateX((nextMousePosition.Y - prevMousePosition.Y) * rotation_constant * (float)delta);
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

    public void _onButtonClearPressed()
    {
        foreach (var c in vehicle.GetChildren())
        {
            c.QueueFree();
        }
    }

    public void _onButtonCombinePressed()
    {
        var scene = GD.Load<PackedScene>("res://vehicles/combine.tscn");
        var instance = scene.Instantiate() as Node3D;
        Container.AddChild(instance);
        add_selection_listener(instance);
    }

    public void _onButtonCubePressed()
    {
        var scene = GD.Load<PackedScene>("res://vehicles/cube.tscn");
        var instance = scene.Instantiate() as Node3D;
        var x = GD.RandRange(-10, 10);
        var y = GD.RandRange(-10, 10);
        var z = GD.RandRange(-10, 10);
        Container.AddChild(instance);
        instance.Scale = new Vector3(0.3f, 0.3f, 0.3f);
        instance.Position = new Vector3(x, y, z);
        add_selection_listener(instance);
    }

    private void override_material(Node3D container)
    {
        foreach (var c in container.ChildrenRecursive())
        {

            var mesh_instance = c as MeshInstance3D;
            if (mesh_instance != null)
            {
                if (mesh_instance.Mesh.GetSurfaceCount() > 0)
                {
                    var material = mesh_instance.Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
                    material.AlbedoColor = ColourPicker.Color;
                    var shader = new ShaderMaterial
                    {
                        Shader = highlightedShader
                    };
                    shader.SetShaderParameter("outline_color", Colors.White);

                    mesh_instance.MaterialOverlay = shader;
                }
            }
        }
    }

    private void remove_override_material(Node3D container)
    {
        foreach (var c in container.ChildrenRecursive())
        {
            var mesh_instance = c as MeshInstance3D;
            if (mesh_instance != null)
            {
                mesh_instance.MaterialOverlay = null;
            }
        }
    }

    private void add_selection_listener(Node3D root)
    {

        foreach (var c in root.ChildrenRecursive())
        {
            var collision = c as CollisionObject3D;
            if (collision != null)
            {
                collision.InputEvent += (Node _camera, InputEvent inputEvent, Vector3 _position, Vector3 _normal, long _shapeIdx) => { _model_clicked(root, inputEvent); };
            }

            var rigid_body = c as RigidBody3D;
            if (rigid_body != null)
            {

                rigid_body.Freeze = true;
            }
        }
    }

    private void freezeNode(Node3D container)
    {

        foreach (var c in container.ChildrenRecursive())
        {
            var rigid_body = c as RigidBody3D;
            if (rigid_body != null)
            {
                rigid_body.Freeze = true;
            }
        }
        var rigid_body_container = container as RigidBody3D;
        if (rigid_body_container != null)
        {
            rigid_body_container.Freeze = true;
        }
    }

    private void _on_color_picker_color_changed(Color _color)
    {

        override_material(Container);
    }

    private void _model_clicked(Node3D source_node, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion)
        {
            return;
        }

        if (inputEvent is InputEventMouseButton)
        {

            var button_event = inputEvent as InputEventMouseButton;
            if (button_event.ButtonIndex == MouseButton.Left && button_event.IsReleased())
            {

                clearSelected();
                override_material(source_node);
                source_node.AddToGroup(SELECTED_GRP);
            }
        }
    }

    private void clearSelected()
    {

        foreach (var n in Container.GetTree().GetNodesInGroup(SELECTED_GRP))
        {
            n.RemoveFromGroup(SELECTED_GRP);
            remove_override_material(n as Node3D);
        }
    }

    private void partButtonPressed(VehiclePart part)
    {
        GD.Print($"Part button {part.Name} pressed.");
        if (selectedParts.Count == 0 && part.PartType != PartType.Body)
        {
            GD.Print("Part added with no existing body, aborting.");
            return;
        }

        if (part.PartType == PartType.Wheels)
        {
            var body = SelectedBody;
            foreach (var wheelAnchor in body.WheelAnchors)
            {
                var wheel = new VehicleWheel3D();
                var instance = part.InstantiateScene();
                wheel.AddChild(instance);
                wheel.Position = wheelAnchor.AttachmentPoint;
                wheel.Rotation = wheelAnchor.BaseRotation;
                wheel.UseAsSteering = wheelAnchor.IsSteering;
                wheel.UseAsTraction = wheelAnchor.IsTraction;
                vehicle.AddChild(wheel);
            }
        }
        else
        {
            var instance = part.InstantiateScene();
            vehicle.AddChild(instance);
        }

        freezeNode(vehicle);
        if (part.PartType == PartType.Body || part.PartType == PartType.Wheels)
        {
            RemoveSelectedPartOfType(part.PartType);
        }
        vehicle.RebuildWheels();
        selectedParts.Add(part);
    }

    private void RemoveSelectedPartOfType(PartType part_type)
    {
        var parts = new List<VehiclePart>();
        foreach (var part in selectedParts)
        {
            if (part.PartType != part_type)
            {
                parts.Add(part);
                // TODO somehow need to delete the thing from the scene tree
            }
        }
        selectedParts = parts;
    }

    public BodyPart SelectedBody
    {
        get
        {
            var part = selectedParts.FirstOrDefault(part => part.PartType == PartType.Body) as BodyPart;
            if (part == null)
            {
                GD.PushWarning("Selected body requested but none has been set.");
            }
            return part;
        }
    }
}
