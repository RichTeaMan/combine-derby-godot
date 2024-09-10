using System;
using System.Threading.Tasks;
using Godot;

public partial class TransitionsCs : CanvasLayer
{
    public static TransitionsCs Current => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<TransitionsCs>("/root/TransitionsCs");

    [Export]
    public int XXXX {get;set;} = 7;

    private AnimationPlayer animationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

    public async Task Fade()
    {
        animationPlayer.Play("fade");
        await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }

        public async Task FadeFuncx(Callable callback_function, Variant[] args)
    {
        animationPlayer.Play("fade");
        await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        callback_function.Call(args);
    }

    public void FadeBack()
    {
        animationPlayer.PlayBackwards("fade");
    }
}
