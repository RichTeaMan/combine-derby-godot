using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class GameSelectScreen : Control
{
    private CheckButton buttonArena => GetNode<CheckButton>("%button_arena");

    private CheckButton buttonHarvest => GetNode<CheckButton>("%button_harvest");

    private VehicleSelect vehicleSelect1 => GetNode<VehicleSelect>("%vehicle_select_1");

    private VehicleSelect vehicleSelect2 => GetNode<VehicleSelect>("%vehicle_select_2");

    private int readyPlayers = 0;

    private int players = 1;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        vehicleSelect1.OnPlayRequested += async (vehicleSelect) => { await onVehicleSelected(vehicleSelect); };
        vehicleSelect2.OnPlayRequested += async (vehicleSelect) => { await onVehicleSelected(vehicleSelect); };
        // vehicle 1 is already joined
        vehicleSelect2.OnPlayerJoined += (sender) => { players++; };

        buttonArena.ButtonPressed = true;
        buttonHarvest.ButtonPressed = false;

        buttonArena.Pressed += () => { gameTypePressed(buttonArena); };
        buttonHarvest.Pressed += () => { gameTypePressed(buttonHarvest); };
    }

    private void gameTypePressed(CheckButton button)
    {

        buttonArena.ButtonPressed = false;
        buttonHarvest.ButtonPressed = false;

        button.ButtonPressed = true;
    }

    private async Task onVehicleSelected(VehicleSelect vehicleSelect)
    {
        readyPlayers++;
        if (readyPlayers == players)
        {
            var players = new List<Player>();
            if (vehicleSelect1.PlayerJoined)
            {
                players.Add(new Player(1, vehicleSelect1.SelectedVehicleName));
            }
            if (vehicleSelect2.PlayerJoined)
            {
                players.Add(new Player(2, vehicleSelect2.SelectedVehicleName));
            }
            await TransitionsCs.Current.Fade();
            GlobalCs.Current.CreateGame(players.ToArray(), FetchCheckedGameMode(), FetchCheckedArenaName());
            QueueFree();
            TransitionsCs.Current.FadeBack();
            GD.Print("Game created");
        }
    }

    private string FetchCheckedArenaName()
    {
        if (buttonArena.ButtonPressed)
        {
            return GlobalCs.ARENA_CRASH;
        }
        if (buttonHarvest.ButtonPressed)
        {
            return GlobalCs.ARENA_FARM;
        }
        return GlobalCs.ARENA_CRASH;
    }

    private string FetchCheckedGameMode()
    {
        if (buttonArena.ButtonPressed)
        {
            return GlobalCs.MODE_POINTS;
        }
        if (buttonHarvest.ButtonPressed)
        {
            return GlobalCs.MODE_HARVEST;
        }
        return GlobalCs.MODE_POINTS;
    }
}
