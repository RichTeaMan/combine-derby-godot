using System.Linq;
using Godot;

public partial class Engine1SoundPartInstance : GeneralEngineSoundPartInstance
{
    public override string FetchEngineNoiseResourcePath()
    {
        return "res://assets/sounds/engine_heavy_loop.ogg";
    }

    public override string[] FetchLargeNoiseResourcePaths()
    {
        string template = "res://assets/sounds/crash/big/$SOUND.ogg";
        return new string[] { "06", "07", "08", "09", "10", "11", "12" }.Select(e => template.Replace("$SOUND", e)).ToArray();
    }

    public override string[] FetchSmallNoiseResourcePaths()
    {
        string template = "res://assets/sounds/crash/small/$SOUND.ogg";
        return new string[] { "03", "04", "05" }.Select(e => template.Replace("$SOUND", e)).ToArray();
    }

}

public class Engine1SoundPart : ISoundPart
{
    public ISoundPartInstance Instantiate()
    {
        GD.Print("ENGINE 1 SOUNDS INSTANTIATED");
        return new Engine1SoundPartInstance();
    }
}
