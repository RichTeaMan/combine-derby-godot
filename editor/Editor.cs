using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public partial class Editor : Node3D
{

    private bool rotating = false;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;
    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;

    const string SELECTED_GRP = "selected";

    private Dictionary<VehiclePart, Control> partControlPairs = new();

    private List<VehiclePart> selectedParts = new();

    private CdVehicle vehicle;

    private Shader highlightedShader;

    private Node3D Container => GetNode<Node3D>("%container");

    private Node3D TestContainer => GetNode<Node3D>("%test-container");

    private VBoxContainer PartsContainer => GetNode<VBoxContainer>("%parts_container");

    private VBoxContainer MaterialContainer => GetNode<VBoxContainer>("%material_container");

    private Node3D Gimbal => GetNode<Node3D>("%gimbal");

    private Camera3D Camera => GetNode<Camera3D>("%camera");

    private ColorPicker ColourPicker => GetNode<ColorPicker>("%ColorPicker");

    private CanvasLayer CanvasLayer => GetNode<CanvasLayer>("%gui");

    private Button BodyPartButton => GetNode<Button>("%button_body_filter");

    private Button WheelPartButton => GetNode<Button>("%button_wheel_filter");

    private Button EnginePartButton => GetNode<Button>("%button_engine_filter");

    private Button AttachmentsPartButton => GetNode<Button>("%button_attachments_filter");

    private Button TestVehicleButton => GetNode<Button>("%button_test_vehicle");

    private Button BuildButtonButton => GetNode<Button>("%button_build_mode");

    private Label PartMass => GetNode<Label>("%part_mass");

    private Label PartDescription => GetNode<Label>("%part_description");

    private Label MassLabel => GetNode<Label>("%label_mass");

    private bool buildMode = true;

    public override void _Ready()
    {
        base._Ready();
        highlightedShader = GD.Load<Shader>("res://shaders/highlighted.gdshader");

        vehicle = new CdVehicle
        {
            Freeze = true
        };

        BodyPartButton.Pressed += () => { FilterPressed(PartType.Body); };
        WheelPartButton.Pressed += () => { FilterPressed(PartType.Wheels); };
        EnginePartButton.Pressed += () => { FilterPressed(PartType.Engine); };
        AttachmentsPartButton.Pressed += () => { FilterPressed(PartType.Attachment); };

        Container.AddChild(vehicle);

        GD.Print("Getting parts...");
        var parts = VehiclePart.PartsInit();
        foreach (var part in parts)
        {
            var img = Image.LoadFromFile(part.ImageUri ?? "res://assets/unknown.png");
            var tex = ImageTexture.CreateFromImage(img);
            var imageButton = new TextureButton
            {
                TextureNormal = tex,
                CustomMinimumSize = new Vector2(128.0f, 128.0f),
                IgnoreTextureSize = true,
                StretchMode = TextureButton.StretchModeEnum.Scale,
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
                SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
            };
            imageButton.Pressed += () => { partButtonPressed(part); };
            imageButton.FocusEntered += () => { partHovered(part); };
            imageButton.MouseEntered += () => { partHovered(part); };
            var partButton = new Button
            {
                Text = $"{part.Name}\n\n{part.Description}",
                CustomMinimumSize = new Vector2(512.0f, 128.0f),
                TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis
            };
            partButton.Pressed += () => { partButtonPressed(part); };
            partButton.FocusEntered += () => { partHovered(part); };
            partButton.MouseEntered += () => { partHovered(part); };
            var row = new HBoxContainer();
            row.AddChild(imageButton);
            row.AddChild(partButton);
            foreach (var data in part.FetchEditorDataCells())
            {
                var label = new Label()
                {
                    Text = data
                };
                row.AddChild(label);
            }

            partControlPairs.Add(part, row);
        }

        FilterPressed(PartType.Body);
        BodyPartButton.GrabFocus();
        resetGui();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var pair in partControlPairs)
        {
            pair.Value.QueueFree();
        }
        base.Dispose(disposing);
    }

    private void FilterPressed(PartType partType)
    {
        foreach (var control in PartsContainer.GetChildren())
        {
            PartsContainer.RemoveChild(control);
        }

        foreach (var pair in partControlPairs.Where(pair => pair.Key.PartType == partType))
        {
            PartsContainer.AddChild(pair.Value);
        }
    }

    private void partHovered(VehiclePart vehiclePart)
    {
        PartDescription.Text = vehiclePart.Description;
        PartMass.Text = $"{vehiclePart.Mass} kg";
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
        if (Input.IsActionJustReleased("ui_alt"))
        {
            if (buildMode)
            {
                _onButtonTestVehiclePressed();
            }
            else
            {
                _onButtonBuildModePressed();
            }
        }
        if (Input.IsActionJustPressed("ui_confirm"))
        {
            var focused = GetViewport().GuiGetFocusOwner();
            if (focused != null)
            {
                focused.EmitSignal("pressed");
            }
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

    private void resetGui()
    {
        CanvasLayer.ChildrenRecursive()
            .Select(n => n as Control)
            .Where(n => n != null)
            .Where(n => n is not Godot.Container || n is ColorPicker)
            .ToList()
            .ForEach(n => n.Visible = buildMode);
        Gimbal.Visible = buildMode;
        TestVehicleButton.Visible = buildMode;
        BuildButtonButton.Visible = !buildMode;
    }

    private void RebuildMaterialContainer()
    {
        foreach (var c in MaterialContainer.GetChildren())
        {
            c.Free();
        }
        foreach (var c in Container.ChildrenRecursive())
        {
            if (c is MeshInstance3D meshInstance)
            {
                foreach (var material in Enumerable.Range(0, meshInstance.Mesh.GetSurfaceCount()).Select(ie => meshInstance.Mesh.SurfaceGetMaterial(ie)))
                {
                    string colour = "unknown";
                    if (material is StandardMaterial3D stanMaterial)
                    {
                        colour = stanMaterial.AlbedoColor.ToString();
                    }
                    Label label = new Label();
                    label.Text = $"{c.Name}: [{colour}]";

                    MaterialContainer.AddChild(label);
                }
            }
        }
    }

    private void overrideMaterial(Node3D container)
    {
        foreach (var c in container.ChildrenRecursive())
        {
            if (c is MeshInstance3D meshInstance)
            {
                if (meshInstance.Mesh.GetSurfaceCount() > 0)
                {
                    var material = meshInstance.Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
                    material.AlbedoColor = ColourPicker.Color;
                    var shader = new ShaderMaterial
                    {
                        Shader = highlightedShader
                    };
                    shader.SetShaderParameter("outline_color", Colors.White);

                    meshInstance.MaterialOverlay = shader;
                }
            }
        }
    }

    private void remove_override_material(Node3D container)
    {
        foreach (var c in container.ChildrenRecursive())
        {
            var meshInstance = c as MeshInstance3D;
            if (meshInstance != null)
            {
                meshInstance.MaterialOverlay = null;
            }
        }
    }

    private void add_selection_listener(Node3D root)
    {

        foreach (var c in root.ChildrenRecursive())
        {
            if (c is CollisionObject3D collision)
            {
                collision.InputEvent += (Node _camera, InputEvent inputEvent, Vector3 _position, Vector3 _normal, long _shapeIdx) => { _model_clicked(root, inputEvent); };
            }

            var rigidBody = c as RigidBody3D;
            if (rigidBody != null)
            {
                rigidBody.Freeze = true;
            }
        }
    }

    private void freezeNode(Node3D container)
    {

        foreach (var c in container.ChildrenRecursive())
        {
            if (c is RigidBody3D rigid_body)
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

        overrideMaterial(Container);
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
                overrideMaterial(source_node);
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

        if (part.PartType == PartType.Body || part.PartType == PartType.Wheels)
        {
            RemoveSelectedPartOfType(part.PartType);
        }
        if (part is BodyPart bodyPart)
        {
            vehicle.Mass = bodyPart.Mass;
        }
        selectedParts.Add(part);

        rebuildFromParts();
    }

    private void rebuildFromParts()
    {

        vehicle.GetChildren().ToList().ForEach(c => c.Free());
        float mass = 0;

        foreach (var selectedPart in selectedParts)
        {
            if (selectedPart.PartType == PartType.Wheels)
            {
                var body = SelectedBody;
                var wheelPart = (WheelPart)selectedPart;
                foreach (var wheelAnchor in body.WheelAnchors)
                {
                    var wheel = new VehicleWheel3D();
                    var instance = selectedPart.InstantiateScene();
                    wheel.AddChild(instance);
                    wheel.Position = wheelAnchor.AttachmentPoint;
                    instance.Rotation = wheelAnchor.BaseRotation;
                    wheel.UseAsSteering = wheelAnchor.IsSteering;
                    wheel.UseAsTraction = wheelAnchor.IsTraction;
                    wheel.SuspensionStiffness = 50.0f;
                    wheel.WheelRadius = wheelPart.Radius;
                    GD.Print($"friction: {wheel.WheelFrictionSlip}");
                    vehicle.AddChild(wheel);
                    mass += selectedPart.Mass;
                }
            }
            else
            {
                var instance = selectedPart.InstantiateScene();
                vehicle.AddChild(instance);
                mass += selectedPart.Mass;
            }
        }
        vehicle.RebuildWheels();
        vehicle.Mass = mass;
        MassLabel.Text = $"{vehicle.Mass} kg";
        freezeNode(vehicle);
        RebuildMaterialContainer();
    }

    private void RemoveSelectedPartOfType(PartType part_type)
    {
        var parts = new List<VehiclePart>();
        foreach (var part in selectedParts)
        {
            if (part.PartType != part_type)
            {
                parts.Add(part);
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

    private void _onButtonTestVehiclePressed()
    {
        if (!buildMode)
        {
            return;
        }
        vehicle.RebuildCamera();
        vehicle.Freeze = false;
        vehicle.ChildrenRecursive()
            .ToList()
            .Select(c => c as RigidBody3D)
            .Where(c => c != null)
            .ToList()
            .ForEach(c => c.Freeze = false);
        Camera.Current = false;
        vehicle.Camera.Current = true;

        vehicle.Reparent(TestContainer);
        vehicle.Transform = Transform3D.Identity;
        buildMode = false;

        resetGui();
    }

    private void _onButtonBuildModePressed()
    {
        if (buildMode)
        {
            return;
        }
        Camera.Current = true;
        vehicle.Camera.Current = false;

        vehicle.Reparent(Container);
        vehicle.Transform = Transform3D.Identity;
        buildMode = true;
        TestVehicleButton.Visible = buildMode;
        BuildButtonButton.Visible = !buildMode;

        resetGui();
        rebuildFromParts();
    }
}
