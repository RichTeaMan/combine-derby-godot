using Godot;

public partial class PlayerViewport : SubViewport
{

	[Export]
	public int player_id { get; set; }

	private bool has_player = false;

	public override void _Ready()
	{
		GlobalCs.Current.PlayerAdded += _on_player_added;
		GlobalCs.Current.GfxSettingsUpdated += _on_gfx_settings_updated;
		GetTree().Root.SizeChanged += _on_resize;
		_on_resize();
		GD.Print($"viewport {player_id} ready.");
	}

	private void _on_resize()
	{
		CallDeferred(nameof(apply_scale));
	}

	private void apply_scale()
	{
		Scaling3DScale = GlobalCs.Current.gfx_scaling;
		GD.Print($"Applied scale {GlobalCs.Current.gfx_scaling}.");
	}

	private void _on_player_added(int added_player_id, Node node)
	{
		if (!has_player && player_id == added_player_id)
		{
			has_player = true;
			AddChild(node);
			GD.Print($"Player {player_id} successfully added.");
		}
	}

	private void _on_gfx_settings_updated()
	{
		apply_scale();
	}
}
