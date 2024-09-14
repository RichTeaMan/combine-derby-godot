using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Harvest : Node3D, IMutliplayerGameType
{

    private Timer TimerNode => GetNode<Timer>("%Timer");

    private Label TimeLabel => GetNode<Label>("%time_label");

    [Export]
    public int PlayerCount { get; set; }

    private const int WHEAT_LIMIT = 5000;

    private const string PROP_WHEAT = "wheat";
    private const string PROP_BALES = "bales";

    private int current_points = 0;
    private int countdown_length_seconds = 60;
    private bool game_active = true;
    private Dictionary<int, PlayerPointsElement> player_points = new Dictionary<int, PlayerPointsElement>();

    private void set_game_info(int player_id)
    {
        int wheat = player_points[player_id].CategoryPoints[PROP_WHEAT];
        var filled = (wheat / WHEAT_LIMIT) * 100.0;
        GlobalCs.Current.SetGameInfoUi(player_id, $"    Wheat storage: {filled}%");
    }

    public override void _Ready()
    {
        refresh_timer();
        GlobalCs.Current.VehiclePickup += on_vehicle_pickup;
        GlobalCs.Current.VehicleBodyShapeEntered += on_vehicle_body_shape_entered;
        foreach (var player_id in Enumerable.Range(0, PlayerCount))
        {
            set_game_info(player_id + 1);
        }
    }

    public override void _EnterTree()
    {
        player_points = new Dictionary<int, PlayerPointsElement>();
        foreach (var i in Enumerable.Range(0, PlayerCount + 1))
        {
            var playerPointsElement = new PlayerPointsElement();
            playerPointsElement.CategoryPoints.Add(PROP_WHEAT, 0);
            playerPointsElement.CategoryPoints.Add(PROP_BALES, 0);
            player_points.Add(i, playerPointsElement);
        }
    }

    private void on_vehicle_pickup(int player_id, string category, int quantity)
    {
        if (category == "wheat" && player_id <= PlayerCount)
        {
            player_points[player_id].CategoryPoints[PROP_WHEAT] += quantity;
            if (player_points[player_id].CategoryPoints[PROP_WHEAT] > WHEAT_LIMIT)
            {
                player_points[player_id].CategoryPoints[PROP_WHEAT] = WHEAT_LIMIT;
            }
            set_game_info(player_id);
        }
    }

    private void on_vehicle_body_shape_entered(int player_id, Node3D body)
    {
        GD.Print("harvest: on vehicle body shape entered");

        if (body is Baler baler)
        {
            GD.Print("deposit wheat");
            baler.DepositWheat(player_points[player_id].CategoryPoints[PROP_WHEAT]);
            player_points[player_id].CategoryPoints[PROP_WHEAT] = 0;
            set_game_info(player_id);
        }
    }

    private void _on_Timer_timeout()
    {
        return;
        /*
        #if countdown_length_seconds <= 0:
        #	game_active = false
        #	$Timer.stop()
        #	var game_over_scene = preload("res://ui/game_over.tscn")

        #	for player_index in player_count:
        #		var player_id = player_index + 1
        #		var game_over_instance = game_over_scene.instantiate()
        #		game_over_instance.player_id = player_id
        #		game_over_instance.points = player_points[player_id]
        #		Global.add_player_ui(player_id, game_over_instance)
        #	return
        #countdown_length_seconds += -1
        #refresh_timer()
        */
    }

    private void refresh_timer()
    {
        var minutes_remaining = countdown_length_seconds / 60;
        var seconds_remaining = countdown_length_seconds % 60;
        TimeLabel.Text = $"[center]{minutes_remaining}:{seconds_remaining}[/center]";
    }

    private class PlayerPointsElement
    {
        public Dictionary<string, int> CategoryPoints { get; set; } = new();
        public int Points { get; set; } = 0;
    }
}