using Godot;
using System;

public partial class GameSelectScreen : Control
{
    private VehicleSelect vehicleSelect1 => GetNode<VehicleSelect>("%vehicle_select_1");
    private VehicleSelect vehicleSelect2 => GetNode<VehicleSelect>("%vehicle_select_2");

    private int readyPlayers = 0;

    private int players = 2;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        vehicleSelect1.PlayRequested += onVehicleSelected;
        vehicleSelect2.PlayRequested += onVehicleSelected;
    }

    private void onVehicleSelected(VehicleSelect vehicleSelect)
    {
        readyPlayers++;
        if (readyPlayers == players)
        {
            QueueFree();
            var players = new Player[] {
                new Player(1, vehicleSelect1.SelectedVehicleName),
                new Player(2, vehicleSelect2.SelectedVehicleName)
            };
            GlobalCs.Current.CreateGame(players, GlobalCs.MODE_POINTS, GlobalCs.ARENA_CRASH);
            GD.Print("Game created");
        }
    }
}
