using System;
using System.Collections.Generic;
using Godot;

public abstract class VehiclePart
{
    public string Name { get; protected set; }
    public PartType PartType { get; protected set; }
    public PackedScene SceneAsset { get; protected set; }

    /// <summary>
    /// The mass, in kg.
    /// </summary>
    public float Mass { get; protected set; } = 1.0f;

    public string Description { get; protected set; }

    public string ImageUri { get; protected set; } = null;

    protected VehiclePart(string name, string scenePath)
    {

        Name = name;
        SceneAsset = GD.Load<PackedScene>(scenePath);
    }

    public Node3D InstantiateScene()
    {
        return (Node3D)SceneAsset.Instantiate();
    }

    /// <summary>
    /// Fetches cells that should be displayed in tabular for for a particular part.
    /// </summary>
    /// <returns></returns>
    public virtual string[] FetchEditorDataCells()
    {
        return Array.Empty<string>();
    }

    public static List<VehiclePart> PartsInit()
    {

        var parts = new List<VehiclePart>();
        parts.AddRange(BodyPart.BodyPartsInit());
        parts.AddRange(WheelPart.WheelPartsInit());
        parts.AddRange(AccessoryPart.AccessoryPartsInit());

        GD.Print("Loaded parts:");
        foreach (var p in parts)
        {
            GD.Print($"{p.PartType} - {p.Name}");
        }
        return parts;
    }
}