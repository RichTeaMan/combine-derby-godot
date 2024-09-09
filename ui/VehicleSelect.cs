using Godot;
using System;
using System.Linq;

public partial class VehicleSelect : Control
{
    [Export]
    public int PlayerId { get; set; } = 1;

    [Export]
    public string SelectedVehicleName { get; set; } = "";

    private string unqiueGroupPrefix = Guid.NewGuid().ToString();

    private string uiGroupName => $"{unqiueGroupPrefix}_VEHICLE_SELECT";

    private VBoxContainer containerUserVehicles => GetNode<VBoxContainer>("%container_user_vehicles");

    private Button buttonEditor => GetNode<Button>("%button_editor");

    private Button buttonPlay => GetNode<Button>("%button_play");

    private SubViewportContainer containerSubViews => GetNode<SubViewportContainer>("%container_sub_views");

    private SubViewport subViewNode => GetNode<SubViewport>("%sub_view");

    private Control selectionRoot => GetNode<Control>("%selection_root");

    private Control finishedSelectionRoot => GetNode<Control>("%finished_selection_root");

    private VehicleLoader vehicleLoader = new VehicleLoader();

    private bool _UiVisible = true;
    public bool UiVisible
    {
        get { return _UiVisible; }
        set
        {
            GetTree().VisibleByGroupName(uiGroupName, value);
            _UiVisible = value;
        }
    }

    public delegate void PlayRequestedHandler(VehicleSelect sender);
    public event PlayRequestedHandler PlayRequested;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (var node in containerSubViews.GetChildren())
        {
            if (node is Control control)
            {
                control.AddToGroup(uiGroupName);
            }
        }
        var vehicleNames = vehicleLoader.LoadUserVehicleNames();
        var theme = ResourceLoader.Load<Theme>("res://themes/ui_theme.tres");
        foreach (var vehicleName in vehicleNames)
        {
            var vehicleButton = new Button()
            {
                Text = vehicleName,
                Theme = theme
            };
            vehicleButton.Pressed += () => { SelectedVehicleName = vehicleName; previewVehicle(); };
            containerUserVehicles.AddChild(vehicleButton);
        }

        buttonEditor.Pressed += openEditor;
        buttonPlay.Pressed += onPlayButton;
        UiVisible = true;
        finishedSelectionRoot.Visible = false;
        previewVehicle();
    }

    private void previewVehicle()
    {
        var displayPodium = GD.Load<PackedScene>("res://arenas/display-podium.tscn");
        var displayPodiumInstance = (Node3D)displayPodium.Instantiate();
        displayPodiumInstance.Position = calcPlayerPositionOffset(displayPodiumInstance.Position);
        subViewNode.GetChildren().ToList().ForEach(n => n.QueueFree());
        subViewNode.AddChild(displayPodiumInstance);
        buttonPlay.Disabled = true;

        if (!string.IsNullOrEmpty(SelectedVehicleName))
        {
            var vehicleModel = vehicleLoader.LoadUserVehicleModelFromName(SelectedVehicleName);
            if (vehicleModel == null)
            {
                return;
            }
            var vehicle = vehicleLoader.LoadUserVehicleFromModel(vehicleModel);
            vehicle.Freeze = true;
            var position = vehicle.Position;
            // TODO: set this better so small vehicles don't float and large ones don't intersect the stage
            position.Y = 1.0f;
            vehicle.Position = position;
            buttonPlay.Disabled = false;
            displayPodiumInstance.AddChild(vehicle);
        }
    }

    private void onPlayButton()
    {
        var modulate = selectionRoot.Modulate;
        modulate.A = 0.5f;
        selectionRoot.Modulate = modulate;

        foreach(var node in selectionRoot.ChildrenRecursive()) {
            if (node is BaseButton button) {
                button.Disabled = true;
            }
        }

        finishedSelectionRoot.Visible = true;

        PlayRequested?.Invoke(this);
    }

    private void openEditor()
    {
        UiVisible = false;
        subViewNode.GetChildren().ToList().ForEach(n => n.QueueFree());
        var editor = GD.Load<PackedScene>("res://editor/editor.tscn");
        var editorInstance = (Editor)editor.Instantiate();
        subViewNode.AddChild(editorInstance);
        editorInstance.Position = calcPlayerPositionOffset(editorInstance.Position);
        editorInstance.LeaveEditorButtonEnabled = true;
        editorInstance.LeaveEditorRequested += () =>
        {
            if (!string.IsNullOrEmpty(editorInstance.VehicleName))
            {
                SelectedVehicleName = editorInstance.VehicleName;
            }
            editorInstance.QueueFree();
            UiVisible = true;
            previewVehicle();
        };
    }

    private Vector3 calcPlayerPositionOffset(Vector3 position)
    {
        position.X = (PlayerId - 1) * 500.0f;
        return position;
    }
}
