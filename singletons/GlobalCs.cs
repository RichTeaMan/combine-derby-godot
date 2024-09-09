using Godot;
using System;
using System.Linq;

public partial class GlobalCs : Node3D
{
    public static GlobalCs Current => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<GlobalCs>("/root/GlobalCs");

    /// <summary>
    /// Original global singleton using GD script.
    /// </summary>
    public static Node Global => ((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/Global");


    public const string ARENA_CRASH = "crash";
    public const string ARENA_FARM = "farm";

    public const string MODE_FREEPLAY = "freeplay";
    public const string MODE_POINTS = "points";
    public const string MODE_HARVEST = "harvest";

    public void CreateGame(Player[] players, string gameMode, string arenaName)
    {
        int playerCount = players.Length;
        GD.Print($"Create game. Players: {playerCount}");

        foreach (var player in GetTree().GetNodesInGroup("player"))
        {
            player.RemoveFromGroup("player");
        }

        PackedScene playerContainer;
        if (playerCount == 1)
        {
            playerContainer = GD.Load<PackedScene>("res://player_frames/one_player.tscn");
        }
        else if (playerCount == 2)
        {
            playerContainer = GD.Load<PackedScene>("res://player_frames/two_player.tscn");
        }
        else
        {
            GD.Print($"Unsupported number of players '{playerCount}'");
            GetTree().Quit();
            return;
        }
        var instance = playerContainer.Instantiate();
        GD.Print("Adding game instance...");
        AddNodeToGlobalRoot(instance);
        
        GD.Print($"Setting up game mode {gameMode}");
        PackedScene gameType;
        if (gameMode == MODE_POINTS)
        {
            gameType = GD.Load<PackedScene>("res://game_rules/points.tscn");
        }
        else if (gameMode == MODE_HARVEST)
        {
            gameType = GD.Load<PackedScene>("res://game_rules/harvest.tscn");
        }
        else
        {
            GD.Print($"Unknown game mode '{gameMode}'");
            GetTree().Quit();
            return;
        }
        var gameInstance = gameType.Instantiate();
        gameInstance.Set("player_count", playerCount);

        PackedScene arena;
        if (arenaName == ARENA_CRASH)
        {
            arena = GD.Load<PackedScene>("res://arenas/crash.tscn");
        }
        else if (arenaName == ARENA_FARM)
        {
            arena = GD.Load<PackedScene>("res://arenas/farm.tscn");
        }
        else
        {
            GD.Print($"Unknown arena '{arenaName}'");
            GetTree().Quit();
            return;
        }
        gameInstance.AddChild(arena.Instantiate());

        var vehicleLoader = new VehicleLoader();
        foreach (var player in players)
        {
            var model = vehicleLoader.LoadUserVehicleModelFromName(player.VehicleName);
            if (model == null)
            {
                GD.Print($"Unknown vehicle '{player.VehicleName}'");
                GetTree().Quit();
                return;
            }
            var vehicleInstance = vehicleLoader.LoadUserVehicleFromModel(model);
            vehicleInstance.PlayerId = player.Id;
            vehicleInstance.AddToGroup("player", true);
            Global.Call("add_player", player.Id, vehicleInstance);
            vehicleInstance.RebuildCamera();
        }
        GD.Print("Combines added");
        instance.AddChild(gameInstance);
        
        //current_game_scene = instance;
        Global.Set("current_game_scene", instance);
        //current_player_count = player_count;
        Global.Set("current_player_count", playerCount);
    }

    public void AddNodeToGlobalRoot(Node node)
    {
        GetParent().AddChild(node);
    }
}
