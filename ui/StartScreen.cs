using System;
using System.Threading.Tasks;
using Godot;

public partial class StartScreen : Control
{

    private Button buttonPlay => GetNode<Button>("%button_play");

    private Button buttonEditor => GetNode<Button>("%button_editor");

    private CheckBox chbSoundEnabled => GetNode<CheckBox>("%chb_sound_enabled");

    private LinkButton buttonGithubLink => GetNode<LinkButton>("%github_link_button");

    public override void _Ready()
    {
        base._Ready();

        buttonPlay.Pressed += async () => { await _onButtonPlayPressed(); };
        buttonEditor.Pressed += async () => { await _onButtonEditorPressed(); };
        buttonGithubLink.Pressed += () => { OS.ShellOpen("https://github.com/RichTeaMan/combine-derby-godot"); };
        chbSoundEnabled.ButtonPressed = !GlobalCs.Current.IsMuted;
        chbSoundEnabled.Toggled += onSoundEnabledChbToggled;
    }

    private void onSoundEnabledChbToggled(bool enabled){
        GlobalCs.Current.IsMuted = !enabled;
    }

    private async Task _onButtonEditorPressed()
    {
        await TransitionsCs.Current.Fade();

        var editor = GD.Load<PackedScene>("res://editor/editor.tscn");
        var editorInstance = editor.Instantiate<Editor>();
        GlobalCs.Current.AddNodeToGlobalRoot(editorInstance);
        // await this ???
        TransitionsCs.Current.FadeBack();
        QueueFree();
    }

    private async Task _onButtonPlayPressed()
    {
        await TransitionsCs.Current.Fade();

        var gameSelect = GD.Load<PackedScene>("res://ui/game_select_screen.tscn");
        var gameSelectInstance = gameSelect.Instantiate<GameSelectScreen>();
        GlobalCs.Current.AddNodeToGlobalRoot(gameSelectInstance);
        // await???
        TransitionsCs.Current.FadeBack();
        QueueFree();
    }

}