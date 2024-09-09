using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics.Tracing;

public partial class Editor : Node3D
{
    private string UNIQUE_PREFIX = Guid.NewGuid().ToString();
    private string EDIT_MODE_GROUP => $"{UNIQUE_PREFIX}-edit_mode_ui";

    private string TEST_MODE_GROUP => $"{UNIQUE_PREFIX}-test_mode_ui";

    private string PARTS_BUTTON_GROUP => $"{UNIQUE_PREFIX}-parts_buttons";

    private bool rotating = false;
    private float rotation_constant = 0.5f;
    private float zoom_constant = 0.2f;
    private Vector2 prevMousePosition;
    private Vector2 nextMousePosition;

    private string SELECTED_GROUP => $"{UNIQUE_PREFIX}-selected";

    private Dictionary<string, VehiclePart> parts = new();

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

    private VBoxContainer FilterContainer => GetNode<VBoxContainer>("%type_filter_buttons");

    private TextEdit VehicleNameTextEdit => GetNode<TextEdit>("%text_vehicle_name");

    private Button SaveButton => GetNode<Button>("%button_save");

    private Button LoadButton => GetNode<Button>("%button_load");

    private Button TestVehicleButton => GetNode<Button>("%button_test_vehicle");

    private Button BuildButtonButton => GetNode<Button>("%button_build_mode");

    private Button LeaveEditorButton => GetNode<Button>("%button_leave_editor");

    private bool _leaveEditorButtonEnabled = false;

    [Export]
    public bool LeaveEditorButtonEnabled
    {
        get { return _leaveEditorButtonEnabled; }
        set
        {
            LeaveEditorButton.Visible = value;
            _leaveEditorButtonEnabled = value;
        }
    }

    public delegate void LeaveEditorRequestedHandler();
    public event LeaveEditorRequestedHandler LeaveEditorRequested;

    private Label MassLabel => GetNode<Label>("%label_mass");

    private Node3D FreePlacementContainer => GetNode<Node3D>("%free-placement-container");

    private bool buildMode = true;

    private bool freePlacement = false;

    private VehiclePart selectedFreePlacementPart = null;

    /// <summary>
    /// Gets the name of named vehicle currently loaded.
    /// 
    /// Note that this is not guarantee that a vehicle was saved.
    /// </summary>
    public string VehicleName => VehicleNameTextEdit.Text;

    public override void _Ready()
    {
        base._Ready();

        SaveButton.Pressed += SavePressed;
        LoadButton.Pressed += LoadPressed;
        LeaveEditorButton.Pressed += leaveEditorButtonPressed;

        highlightedShader = GD.Load<Shader>("res://shaders/highlighted.gdshader");

        vehicle = new CdVehicle
        {
            Freeze = true
        };

        Container.AddChild(vehicle);

        GD.Print("Getting parts...");
        var partsList = VehiclePart.PartsInit();

        foreach (var partType in partsList.Select(p => p.PartType).Distinct().OrderBy(pt => pt))
        {
            var partButton = new Button()
            {
                Text = partType.ToString()
            };
            partButton.Pressed += () => { FilterPressed(partType); };
            FilterContainer.AddChild(partButton);
        }

        foreach (var part in partsList)
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
                SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
                FocusMode = Control.FocusModeEnum.None,
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

            PartsContainer.AddChild(row);
            row.AddToGroup(PARTS_BUTTON_GROUP);
            row.AddToGroup(part.PartType.GroupName(UNIQUE_PREFIX));

            parts.Add(part.Name, part);
        }

        FilterPressed(PartType.Body);
        (FilterContainer.GetChildren().FirstOrDefault(n => n is Button) as Button)?.GrabFocus();
        resetGui();
        MassLabel.Text = "Vehicle mass: 0 kg";

    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    private Calc3dMousePositionResult Calc3dMousePosition()
    {
        var mouse_pos = GetViewport().GetMousePosition();
        var ray_length = 100;
        var from = Camera.ProjectRayOrigin(mouse_pos);
        var to = from + Camera.ProjectRayNormal(mouse_pos) * ray_length;
        var space = GetWorld3D().DirectSpaceState;
        using var ray_query = new PhysicsRayQueryParameters3D()
        {
            From = from,
            To = to,
            CollideWithAreas = false,
            CollideWithBodies = true,
        };
        var raycast_result = space.IntersectRay(ray_query);
        var position = Vector3.Zero;
        var normal = Vector3.Zero;
        bool positionFound = false;
        if (raycast_result.TryGetValue("collider_id", out Variant colliderIdVariant))
        {
            ulong colliderId = colliderIdVariant.AsUInt64();
            if (vehicle.ContainsId(colliderId))
            {
                if (raycast_result.TryGetValue("position", out Variant positionVariant))
                {
                    position = positionVariant.AsVector3();
                    positionFound = true;
                    if (raycast_result.TryGetValue("normal", out Variant normalVariant))
                    {
                        normal = normalVariant.AsVector3();
                    }
                }
            }
        }
        return new Calc3dMousePositionResult
        {
            PositionFound = positionFound,
            Position = position,
            Normal = normal,
        };
    }

    class Calc3dMousePositionResult
    {
        public bool PositionFound { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
    }

    private void FilterPressed(PartType partType)
    {
        GetTree().HideByGroupName(PARTS_BUTTON_GROUP);
        GetTree().ShowByGroupName(partType.GroupName(UNIQUE_PREFIX));
        freePlacement = partType == PartType.Accessory;
    }

    private void SavePressed()
    {
        GD.Print("Saving file...");
        if (string.IsNullOrWhiteSpace(VehicleNameTextEdit.Text))
        {
            GD.Print("Did not save, no vehicle name.");
            return;
        }
        string filepath = $"user://vehicles/{VehicleNameTextEdit.Text}.json";
        GD.Print($"Saving file at {filepath}.");
        DirAccess.MakeDirRecursiveAbsolute("user://vehicles");
        using var saveFile = FileAccess.Open(filepath, FileAccess.ModeFlags.Write);
        vehicle.Model.Name = VehicleNameTextEdit.Text;
        var options = new JsonSerializerOptions { IncludeFields = true };
        string json = JsonSerializer.Serialize(vehicle.Model, options);
        saveFile.StoreLine(json);
        GD.Print(json);
        saveFile.Close();
        GD.Print("File saved.");
    }

    private void LoadPressed()
    {
        FileDialog dialog = new()
        {
            Access = FileDialog.AccessEnum.Userdata,
            CurrentDir = "user://vehicles",
            FileMode = FileDialog.FileModeEnum.OpenFile,
            MinSize = new Vector2I(800, 800),
        };
        AddChild(dialog);
        dialog.Show();

        dialog.FileSelected += (selectedPath) =>
        {
            PerformLoad(selectedPath);
            dialog.QueueFree();
        };
        dialog.Canceled += () =>
        {
            dialog.QueueFree();
        };
    }

    private void PerformLoad(string filepath)
    {
        GD.Print($"Loading {filepath}...");
        var vehicleLoader = new VehicleLoader();
        var model = vehicleLoader.LoadUserVehicleModelFromFilepath(filepath);
        vehicle.Model = model;
        rebuildFromParts();
        VehicleNameTextEdit.Text = model.Name;
        GD.Print("Load complete");
    }

    private void partHovered(VehiclePart vehiclePart)
    {

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

        if (freePlacement && selectedFreePlacementPart != null)
        {
            var positionResult = Calc3dMousePosition();
            if (positionResult.PositionFound)
            {
                FreePlacementContainer.Position = positionResult.Position;
                FreePlacementContainer.Transform = AlignWithY(FreePlacementContainer.Transform, positionResult.Normal);
                FreePlacementContainer.Visible = true;

                if (Input.IsActionJustPressed("left_click"))
                {
                    if (selectedFreePlacementPart is AccessoryPart accessoryPart)
                    {
                        var clone = accessoryPart.InstantiateScene();
                        vehicle.AddChild(clone);
                        clone.GlobalTransform = FreePlacementContainer.GlobalTransform;
                        vehicle.Model.Accessories.Add(
                            new CdVehicle.CdVehicleModelTransform
                            {
                                PartId = accessoryPart.Name,
                                Transform = clone.Transform
                            });
                    }
                }

                if (Input.IsActionPressed("rotate_part_clockwise"))
                {
                    FreePlacementContainer.Rotate(Vector3.Up, -0.1f);
                }

                if (Input.IsActionPressed("rotate_part_anti_clockwise"))
                {
                    FreePlacementContainer.Rotate(Vector3.Up, 0.1f);
                }
            }
            else
            {
                FreePlacementContainer.Visible = false;
            }
        }
    }

    // taken from https://kidscancode.org/godot_recipes/3.x/3d/3d_align_surface/index.html
    private Transform3D AlignWithY(Transform3D xform, Vector3 new_y)
    {
        xform.Basis.Y = new_y;
        xform.Basis.X = -xform.Basis.Z.Cross(new_y);
        xform.Basis = xform.Basis.Orthonormalized();
        return xform;
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
        if (buildMode)
        {
            GetTree().HideByGroupName(TEST_MODE_GROUP);
            GetTree().ShowByGroupName(EDIT_MODE_GROUP);
        }
        else
        {
            GetTree().ShowByGroupName(TEST_MODE_GROUP);
            GetTree().HideByGroupName(EDIT_MODE_GROUP);
        }
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
                source_node.AddToGroup(SELECTED_GROUP);
            }
        }
    }

    private void clearSelected()
    {
        foreach (var n in Container.GetTree().GetNodesInGroup(SELECTED_GROUP))
        {
            n.RemoveFromGroup(SELECTED_GROUP);
            remove_override_material(n as Node3D);
        }
    }

    private void partButtonPressed(VehiclePart part)
    {
        GD.Print($"Part button {part.Name} pressed.");
        FreePlacementContainer.ChildrenRecursive().ForEach(c => c.QueueFree());
        selectedFreePlacementPart = null;

        if (part.PartType == PartType.Body)
        {
            vehicle.Model.BodyId = part.Name;
        }
        else if (part.PartType == PartType.Wheels)
        {
            vehicle.Model.WheelsId = part.Name;
        }

        if (freePlacement)
        {
            selectedFreePlacementPart = part;
            FreePlacementContainer.AddChild(part.InstantiateScene());
        }
        else
        {
            rebuildFromParts();
        }
    }

    private void rebuildFromParts()
    {

        vehicle.RebuildFromParts(parts);
        freezeNode(vehicle);
        RebuildMaterialContainer();
        MassLabel.Text = $"Vehicle mass: {vehicle.Mass} kg";
    }

    private void _onButtonTestVehiclePressed()
    {
        if (!buildMode || vehicle.Model.BodyId == null)
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

        resetGui();
        rebuildFromParts();
    }

    private void leaveEditorButtonPressed()
    {
        GD.Print($"leave? {LeaveEditorButtonEnabled}");

        if (!LeaveEditorButtonEnabled)
        {
            return;
        }
        LeaveEditorRequested?.Invoke();
    }
}
