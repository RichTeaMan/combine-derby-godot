using System;
using System.Collections.Generic;
using Godot;

public enum PartType
{
    Body,
    Wheels,
    Engine,
    Attachment
}

public abstract class VehiclePart
{
    public string Name { get; protected set; }
    public PartType PartType { get; protected set; }
    public PackedScene SceneAsset { get; protected set; }

    protected VehiclePart(string name, string scenePath)
    {

        Name = name;
        SceneAsset = GD.Load<PackedScene>(scenePath);
    }

    public Node3D InstantiateScene()
    {
        return (Node3D)SceneAsset.Instantiate();
    }

    public static List<VehiclePart> PartsInit()
    {

        var parts = new List<VehiclePart>();
        parts.AddRange(BodyPart.BodyPartsInit());
        parts.AddRange(WheelPart.WheelPartsInit());

        GD.Print("Loaded parts:");
        foreach (var p in parts)
        {
            GD.Print($"{p.PartType} - {p.Name}");
        }
        return parts;
    }
}