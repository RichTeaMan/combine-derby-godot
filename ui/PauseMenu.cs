using Godot;

public partial class PauseMenu : MarginContainer
{

    private bool preloaded = false;

    private Button buttonRestart => GetNode<Button>("%button_restart");

    private Button buttonQuit => GetNode<Button>("%button_quit");

    private Slider sliderGfx => GetNode<Slider>("%gfx_scaling_slider");

    private Slider sliderMasterVolume => GetNode<Slider>("%master_volume_slider");

    private Slider sliderMusicVolume => GetNode<Slider>("%music_volume_slider");

    private Slider sliderSfxVolume => GetNode<Slider>("%sfx_volume_slider");

    private CheckBox chbReverseCamera => GetNode<CheckBox>("%checkbox_reverse_camera");

    public override void _Ready()
    {
        GD.Print($"ready, {GlobalCs.Current.gfx_scaling}");
        preloaded = true;

        GetTree().VisibleByGroupName("html5_disable", !GlobalCs.Current.IsWeb);

        buttonQuit.Pressed += _on_button_quit_pressed;
        buttonRestart.Pressed += _on_button_restart_pressed;

        sliderGfx.ValueChanged += _on_gfx_scaling_slider_value_changed;
        sliderGfx.Value = GlobalCs.Current.gfx_scaling;

        chbReverseCamera.GuiInput += _on_checkbox_reverse_camera_gui_input;
        chbReverseCamera.ButtonPressed = !GlobalCs.Current.CameraReverses;

        sliderMasterVolume.ValueChanged += _on_master_volume_slider_value_changed;
        sliderMasterVolume.Value = GlobalCs.Current.GetMasterVolume();

        sliderMusicVolume.ValueChanged += _on_music_volume_slider_value_changed;
        sliderMusicVolume.Value = GlobalCs.Current.GetMusicVolume();

        sliderSfxVolume.ValueChanged += _on_sfx_volume_slider_value_changed;
        sliderSfxVolume.Value = GlobalCs.Current.GetSfxVolume();
    }

    public override void _UnhandledInput(InputEvent _inputEvent)
    {
        if (!GlobalCs.Current.IsWeb && Input.IsActionJustPressed("menu_quit"))
        {
            GD.Print("Quit key pressed");
            GetTree().Quit();
        }
    }

    private void _on_checkbox_reverse_camera_gui_input(InputEvent _event)
    {
        GlobalCs.Current.CameraReverses = !chbReverseCamera.ButtonPressed;
    }


    private void _on_gfx_scaling_slider_value_changed(double _value)
    {
        if (!preloaded)
        {
            return;
        }
        GD.Print($"gfx range {sliderGfx.Value}");

        GlobalCs.Current.gfx_scaling = (float)sliderGfx.Value;
        GlobalCs.Current.DoGfxSettingsUpdated();
    }

    private void _on_master_volume_slider_value_changed(double _value)
    {
        if (!preloaded)
        {
            return;
        }
        GD.Print($"master volume range {sliderMasterVolume.Value}");

        GlobalCs.Current.SetMasterVolume((float)sliderMasterVolume.Value);
    }

    private void _on_music_volume_slider_value_changed(double _value)
    {
        if (!preloaded)
        {
            return;
        }
        GD.Print($"music volume range {sliderMusicVolume.Value}");

        GlobalCs.Current.SetMusicVolume((float)sliderMusicVolume.Value);
    }

    private void _on_sfx_volume_slider_value_changed(double _value)
    {
        if (!preloaded)
        {
            return;
        }
        GD.Print($"sfx volume range {sliderSfxVolume.Value}");

        GlobalCs.Current.SetSfxVolume((float)sliderSfxVolume.Value);
    }

    private void _on_button_restart_pressed()
    {
        GlobalCs.Current.ClosePause();

        GlobalCs.Current.do_restart_game();

        TransitionsCs.Current.FadeBack();
    }

    private void _on_button_quit_pressed()
    {
        GD.Print("Quit button pressed");
        GetTree().Quit();
    }
}
