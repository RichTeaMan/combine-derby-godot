using System.Threading;
using Godot;

public enum PartType
{
    Body,
    Wheels,
    Engine,
    Attachment,

    Accessory
}

public static class PartTypeUtility
{
    public static string GroupName(this PartType type)
    {
        return $"{type}_PART_ GROUP";
    }
}