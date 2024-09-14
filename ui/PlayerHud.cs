using Godot;

public partial class PlayerHud : Node
{

	private int countdown_length_seconds = 60;
	private bool game_active = true;

	private RichTextLabel labelControl => GetNode<RichTextLabel>("%control_label");

	private RichTextLabel labelGameInfo => GetNode<RichTextLabel>("%game_info_label");

	private RichTextLabel labelSpeed => GetNode<RichTextLabel>("%speed_label");

	private AnimationPlayer animationPlayer => GetNode<AnimationPlayer>("%animation_player");

	[Export]
	public int PlayerId { get; set; } = 1;

	public override void _Ready()
	{
		GlobalCs.Current.GameInfoUi += on_set_game_info_ui;
		GlobalCs.Current.Speed += _on_speed;
		GlobalCs.Current.PlayerUi += _on_player_ui;
		on_set_game_info_ui(PlayerId, "");
		_on_speed(PlayerId, 0);

		string template = "[center]Control with $CONTROLS keys.[/center]";
		string control_text = "";
		if (PlayerId == 1)
		{
			control_text = template.Replace("$CONTROLS", "WASD");
		}
		else if (PlayerId == 2)
		{
			control_text = template.Replace("$CONTROLS", "IJKL");
		}
		labelControl.Text = control_text;
	}

	private void _on_player_ui(int signal_player_id, Node ui)
	{
		GD.Print($"_on_player_ui {signal_player_id}");
		if (signal_player_id != PlayerId)
		{
			return;
		}
		GD.Print($"Received ui for player {PlayerId}");
		AddChild(ui);
	}

	private void on_set_game_info_ui(int signal_player_id, string message)
	{
		if (signal_player_id != PlayerId || !game_active)
		{
			return;
		}
		labelGameInfo.Text = message;
	}

	private void _on_speed(int signal_player_id, float speed)
	{
		if (signal_player_id != PlayerId)
		{
			return;
		}
		labelSpeed.Text = $"Speed: {speed:F1}m/s";
	}

	private void _on_Timer_timeout()
	{
		animationPlayer.Play("control_fade");
	}
}
