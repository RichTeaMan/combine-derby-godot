using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Points : Node3D, IMutliplayerGameType
{


    private Timer TimerNode => GetNode<Timer>("%Timer");

    private RichTextLabel TimeLabel => GetNode<RichTextLabel>("%time_label");


    [Export]
    public int PlayerCount { get; set; }

    private int countdownLengthSeconds = 60;

    private bool game_active = true;

    private Dictionary<int, PlayerPointsElement> playerPoints = new Dictionary<int, PlayerPointsElement>();

    private Dictionary<string, int> pointsMap = new() {
        {"Barrel booms", 20},
        {"Bull bounces",5},
        {"Chickens", 1},
        {"Cow bounces", 5},
        {"Hay bales", 10}
    };

    private void setPoints(int player_id)
    {
        GlobalCs.Current.SetGameInfoUi(player_id, $"    Points: {playerPoints[player_id].Points}");
    }

    public override void _Ready()
    {
        RefreshTimer();
        GlobalCs.Current.VehiclePickup += onVehiclePickup;
        TimerNode.Timeout += onTimerTimeout;
        foreach (var player_id in Enumerable.Range(0, PlayerCount))
        {
            setPoints(player_id + 1);
        }
    }

    public override void _EnterTree()
    {
        playerPoints = new Dictionary<int, PlayerPointsElement>();
        foreach (var i in Enumerable.Range(0, PlayerCount + 1))
        {
            playerPoints.Add(i, new PlayerPointsElement());
        }
    }

    private void onVehiclePickup(int player_id, string category, int quantity)
    {
        if (!game_active || !pointsMap.ContainsKey(category))
        {
            return;
        }
        if (player_id <= PlayerCount)
        {
            if (!playerPoints[player_id].CategoryPoints.ContainsKey(category))
            {
                playerPoints[player_id].CategoryPoints.Add(category, 0);
            }
            var points = pointsMap[category];
            playerPoints[player_id].CategoryPoints[category] += 1;
            playerPoints[player_id].Points += points;
            setPoints(player_id);
        }
    }

    private void onTimerTimeout()
    {
        if (countdownLengthSeconds <= 0)
        {
            GD.Print($"Timeover. Player count: {PlayerCount}");

            game_active = false;
            TimerNode.Stop();
            var game_over_scene = GD.Load<PackedScene>("res://ui/game_over.tscn");

            foreach (var player_index in Enumerable.Range(0, PlayerCount))
            {
                var player_id = player_index + 1;
                var gameOverInstance = game_over_scene.Instantiate<GameOver>();
                gameOverInstance.PlayerId = player_id;
                gameOverInstance.points = playerPoints[player_id].CategoryPoints;
                GlobalCs.Current.AddPlayerUi(player_id, gameOverInstance);
            }
            return;
        }
        countdownLengthSeconds += -1;
        RefreshTimer();
    }

    private void RefreshTimer()
    {
        var minutes_remaining = countdownLengthSeconds / 60;
        var seconds_remaining = countdownLengthSeconds % 60;
        TimeLabel.Text = $"[center]{minutes_remaining:D1}:{seconds_remaining:D2} [/center]";
    }

    private class PlayerPointsElement
    {
        public Dictionary<string, int> CategoryPoints { get; set; } = new();
        public int Points { get; set; } = 0;
    }
}