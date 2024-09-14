using System.Linq;
using Godot;

public partial class Arena : Node3D
{

    private Node3D Combine1 => GetNode<Node3D>("combine");
    private Node3D Combine2 => GetNode<Node3D>("combine2");

    public override void _Ready()
    {
        GD.Print("Setting up arena.");
        GlobalCs.Current.SetPlaylist(
            new string[] {"JasonShaw_JennysTheme.ogg",
        "JasonShaw_Snappy.ogg",
        "jazzyfrenchy.ogg"},
             true);

        var spawns = fetch_player_spawns();
        var players = GetTree().GetNodesInGroup("player").Cast<Node3D>().ToArray();
        GD.Print($"    Adding {players.Length} players.");
        GD.Print($"    Found {spawns.Length} arena spawns.");
        foreach (var i in Enumerable.Range(0, players.Length))
        {
            players[i].Transform = spawns[i];
            GD.Print($"    Set transform for player {i} at {spawns[i]}.");
        }
        Combine1.QueueFree();
        Combine2.QueueFree();
        GD.Print("Arena setup done.");
    }

    private Transform3D[] fetch_player_spawns()
    {
        return new[] { Combine1.Transform, Combine2.Transform };
    }
}
