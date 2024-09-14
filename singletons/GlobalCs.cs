using Godot;
using System;
using System.Linq;

public partial class GlobalCs : Node3D
{
    private Random random = new Random();

    private static GlobalCs _current = null;
    public static GlobalCs Current
    {
        get
        {
            _current ??= ((SceneTree)Engine.GetMainLoop()).Root.GetNode<GlobalCs>("/root/Global");
            return _current;
        }
    }
    private AudioStreamPlayer musicPlayer => GetNode<AudioStreamPlayer>("%music_player");


    public const string ARENA_CRASH = "crash";
    public const string ARENA_FARM = "farm";

    public const string MODE_FREEPLAY = "freeplay";
    public const string MODE_POINTS = "points";
    public const string MODE_HARVEST = "harvest";

    public static StringName VEHICLE_GROUP_NAME = "vehicle";

    private static StringName PlayerAddedSignalName = "PlayerAdded";
    private static StringName VehiclePickupSignalName = "VehiclePickup";
    private static StringName VehicleBodyShapeEnteredSignalName = "VehicleBodyShapeEntered";
    private static StringName GameInfoUiSignalName = "GameInfoUi";
    private static StringName SpeedSignalName = "Speed";
    private static StringName PlayerUiSignalName = "PlayerUi";





    public PauseMenu pause_menu;


    private Node current_game_scene;
    int current_player_count = 1;

    public bool CameraReverses { get; set; } = true;

    string[] current_playlist;
    int current_playlist_index;


    public float gfx_scaling = 0.5f;



    [Signal]
    public delegate void VehiclePickupEventHandler(int player_id, string category, int quantity);

    [Signal]
    public delegate void VehicleBodyShapeEnteredEventHandler(int player_id, Node3D body);

    [Signal]
    public delegate void GameInfoUiEventHandler(int player_id, string message);

    [Signal]
    public delegate void SpeedEventHandler(int playerId, float speedMs);

    [Signal]
    public delegate void PlayerAddedEventHandler(int player_id, Node3D node);

    [Signal]
    public delegate void PlayerUiEventHandler(int player_id, Node node);

    [Signal]
    public delegate void GfxSettingsUpdatedEventHandler();

    public override void _Ready()
    {
        if (IsWeb)
        {
            gfx_scaling = 0.4f;
        }
        var pause_menu_template = GD.Load<PackedScene>("res://ui/pause_menu.tscn");
        pause_menu = pause_menu_template.Instantiate<PauseMenu>();
        musicPlayer.Finished += onMusicPlayerFinished;
    }

    public void AddPlayer(int player_id, Node node)
    {
        EmitSignal(PlayerAddedSignalName, player_id, node);
    }

    public void DoVehiclePickup(int player_id, string category, int quantity)
    {
        EmitSignal(VehiclePickupSignalName, player_id, category, quantity);
    }

    public void DoVehicleBodyShapeEntered(int player_id, Node3D body)
    {
        EmitSignal(VehicleBodyShapeEnteredSignalName, player_id, body);
    }

    public void SetGameInfoUi(int player_id, string message)
    {
        EmitSignal(GameInfoUiSignalName, player_id, message);
    }

    public void UpdateSpeed(int playerId, float speedMs)
    {
        EmitSignal(SpeedSignalName, playerId, speedMs);
    }


    public void AddPlayerUi(int player_id, Node node)
    {
        EmitSignal(PlayerUiSignalName, player_id, node);
    }

    public void do_restart_game()
    {
        current_game_scene.QueueFree();
        // TODO
        // CallDeferred("create_game", current_player_count, "", "");
    }

    public void DoGfxSettingsUpdated()
    {
        GD.Print("GFX scaling set to {gfx_scaling}.");
        EmitSignal("GfxSettingsUpdated");
    }

    public bool IsInVehicleGroup(Node node)
    {
        return node.IsInGroup(VEHICLE_GROUP_NAME);
    }

    // may not be used?????
    //public void get_screen_width(){
    //	return GetViewport().size.X;
    //}
    //
    //public void get_screen_height(){
    //	return get_viewport().size.y;
    //}

    public override void _UnhandledInput(InputEvent _inputEvent)
    {
        if (Input.IsActionJustPressed("menu"))
        {
            GetTree().Quit();
            return;
            if (GetTree().Paused)
            {
                ClosePause();
            }
            else
            {
                OpenPause();
            }
        }
    }

    public void OpenPause()
    {
        GetTree().Paused = true;
        GetTree().GetRoot().AddChild(pause_menu);
    }

    public void ClosePause()
    {
        GetTree().Paused = false;
        GetTree().GetRoot().RemoveChild(pause_menu);
    }


    public bool IsWeb => OS.GetName() == "HTML5";

    public bool IsMuted
    {
        get
        {
            return AudioServer.IsBusMute(AudioServer.GetBusIndex("Master"));
        }
        set
        {
            GD.Print($"Muting master bus: {value}");
            AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), value);
        }
    }

    // Sets master volume. Volume should between 0.0 and 1.0.
    public void SetMasterVolume(float volume)
    {
        var resolved_db = ResolveVolumeFractionToDb(volume);
        GD.Print($"Master volume set to {volume}, resolved db set to {resolved_db}");
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), resolved_db);
    }

    public float GetMasterVolume()
    {
        var db = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"));
        return ResolveDbVolumeFraction(db);
    }

    // Sets music volume. Volume should between 0.0 and 1.0.
    public void SetMusicVolume(float volume)
    {
        var resolved_db = ResolveVolumeFractionToDb(volume);
        GD.Print($"Music volume set to {volume}, resolved db set to {resolved_db}");
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("music"), resolved_db);
    }

    public float GetMusicVolume()
    {
        var db = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("music"));
        return ResolveDbVolumeFraction(db);
    }

    // Sets sfx volume. Volume should between 0.0 and 1.0.
    public void SetSfxVolume(float volume)
    {
        var resolved_db = ResolveVolumeFractionToDb(volume);
        GD.Print($"SFX volume set to {volume}, resolved db set to {resolved_db}");
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("sfx"), resolved_db);
    }

    public float GetSfxVolume()
    {
        var db = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("sfx"));
        return ResolveDbVolumeFraction(db);
    }

    public float ResolveVolumeFractionToDb(float volume)
    {
        return Mathf.LinearToDb(volume);
    }

    public float ResolveDbVolumeFraction(float db)
    {
        return Mathf.DbToLinear(db);
    }

    public void PlayMusic(string name)
    {
        var file = "res://assets/music/{name}";
        if (ResourceLoader.Exists(file))
        {
            GD.Print($"Playing {name}");
            var music = GD.Load<AudioStreamOggVorbis>(file);
            music.Loop = false;
            musicPlayer.Stream = music;
            musicPlayer.Play();
        }
        else
        {
            GD.Print($"Unable to play {name}, file not found");
        }
    }

    public void SetPlaylist(string[] playlist, bool random_start = false)
    {
        if (playlist != null && playlist.Length > 0)
        {
            current_playlist = playlist;
            current_playlist_index = 0;
            if (random_start)
            {
                current_playlist_index = random.Next() % playlist.Length;
            }
            PlayMusic(current_playlist[current_playlist_index]);
        }
    }

    public void onMusicPlayerFinished()
    {
        if (current_playlist == null || current_playlist.Length == 0)
        {
            return;
        }
        current_playlist_index += 1;
        if (current_playlist_index == current_playlist.Length)
        {
            current_playlist_index = 0;
        }
        PlayMusic(current_playlist[current_playlist_index]);
    }
    public void LoadStartScreen()
    {
        var scene = GD.Load<PackedScene>("res://ui/start_screen.tscn");
        var start = scene.Instantiate<StartScreen>();
        AddChild(start);
    }

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
        var gameInstance = gameType.Instantiate<Node3D>();
        if (gameInstance is IMutliplayerGameType multiGame) {
            multiGame.PlayerCount = playerCount;
        }

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
            AddPlayer(player.Id, vehicleInstance);
            vehicleInstance.RebuildCamera();
        }
        GD.Print("Combines added");
        instance.AddChild(gameInstance);

        current_game_scene = instance;
        current_player_count = playerCount;
    }

    public void AddNodeToGlobalRoot(Node node)
    {
        GetParent().AddChild(node);
    }
}
