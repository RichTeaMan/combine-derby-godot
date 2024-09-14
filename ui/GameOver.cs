using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class GameOver : CenterContainer
{

	[Export]
	public int PlayerId { get; set; }

	public Dictionary<string, int> points { get; set; }

	private RichTextLabel pointsLabel => GetNode<RichTextLabel>("%points_label");

	private Button buttonRestart => GetNode<Button>("%restart_button");

	private Button buttonResume => GetNode<Button>("%resume_button");

	private GridContainer pointsBreakdownGrid => GetNode<GridContainer>("%points_breakdown_grid");

	public override void _Ready()
	{
		buttonRestart.Pressed += async () => { await _on_restart_button_pressed(); };
		buttonResume.Pressed += _on_resume_button_pressed;

		pointsLabel.Text = $"Points {points["points"]}";
		var row_height = 50;
		var key_cell_width = 180;
		var value_cell_width = 40;
		foreach (var kv in points)
		{
			var p = kv.Key;
			var v = kv.Value;
			if (p == "points")
			{
				continue;
			}
			var key_label = new RichTextLabel
			{
				Text = p,
				CustomMinimumSize = new Vector2()
				{
					X = key_cell_width,
					Y = row_height
				},
				Theme = pointsLabel.Theme
			};
			key_label.AddThemeFontSizeOverride("normal_font_size", 24);
			var value_label = new RichTextLabel
			{
				Text = v.ToString(),
				CustomMinimumSize = new Vector2()
				{
					X = value_cell_width,
					Y = row_height
				},
				Theme = pointsLabel.Theme
			};
			value_label.AddThemeFontSizeOverride("normal_font_size", 24);
			pointsBreakdownGrid.AddChild(key_label);
			pointsBreakdownGrid.AddChild(value_label);
		}
	}

	private void _on_resume_button_pressed()
	{
		GetTree().HideByGroupName("points_ui");
		QueueFree();
	}

	private async Task _on_restart_button_pressed()
	{
		await TransitionsCs.Current.Fade();
		GlobalCs.Current.do_restart_game();
		TransitionsCs.Current.FadeBack();
	}
}
