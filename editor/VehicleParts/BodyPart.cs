using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BodyPart : VehiclePart
{

    public IReadOnlyList<WheelAnchor> WheelAnchors => _wheelAnchors.AsReadOnly();

    public float BaseRpm { get; protected set; } = 500.0f;

    public float BaseTorque { get; protected set; } = 2000.0f;

    public ISoundPart SoundPart { get; set; } = null;


    private List<WheelAnchor> _wheelAnchors = new();

    public BodyPart(string name, string scenePath) : base(name, scenePath)
    {
        PartType = PartType.Body;
    }

    public void AddWheelAnchor(Vector3 anchorPoint, bool isTraction, bool isSteering)
    {
        var anchor = new WheelAnchor(anchorPoint, isTraction, isSteering);
        _wheelAnchors.Add(anchor);
    }

    public Vector3 CalculateCenterOfMass()
    {
        if (!WheelAnchors.Any())
        {
            return Vector3.Zero;
        }

        return new Vector3(0.0f, WheelAnchors.Average(wa => wa.AttachmentPoint.Y), 0.0f);
    }

    public override string[] FetchEditorDataCells()
    {
        return new[] { $"{Mass} kg" };
    }

    public static List<BodyPart> BodyPartsInit()
    {
        var parts = new List<BodyPart>();

        // silo
        var silo = new BodyPart("Silo", "res://assets/parts/body/silo-body.tscn")
        {
            Description = "An old corn silo, bravely and tragically repurposed into a chassis.",
            Mass = 800.0f,
            BaseRpm = 500.0f,
            BaseTorque = 2000.0f,
            ImageUri = "res://assets/parts/body/silo-body-icon.png",
            SoundPart = new Engine1SoundPart(),
        };
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, 3.0f), false, true);
        silo.AddWheelAnchor(new Vector3(1.5f, -0.8f, -3.0f), true, false);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, 3.0f), false, true);
        silo.AddWheelAnchor(new Vector3(-1.5f, -0.8f, -3.0f), true, false);
        parts.Add(silo);

        // hay
        var hay = new BodyPart("Hay Bale", "res://assets/parts/body/hay-body.tscn")
        {
            Description = "Just a hollowed out bale of hay.",
            BaseRpm = 500.0f,
            BaseTorque = 800.0f,
            Mass = 100.0f,
        };
        hay.AddWheelAnchor(new Vector3(1.0f, -0.5f, 0.6f), false, true);
        hay.AddWheelAnchor(new Vector3(1.0f, -0.5f, -0.6f), true, false);
        hay.AddWheelAnchor(new Vector3(-1.0f, -0.5f, 0.6f), false, true);
        hay.AddWheelAnchor(new Vector3(-1.0f, -0.5f, -0.6f), true, false);

        parts.Add(hay);

        // combine
        var combine = new BodyPart("Combine Harvester", "res://assets/parts/body/combine-body.tscn")
        {
            Description = "The thing from the title screen!",
            BaseRpm = 1000.0f,
            BaseTorque = 2000.0f,
            Mass = 800.0f,
            SoundPart = new Engine1SoundPart(),
        };
        combine.AddWheelAnchor(new Vector3(1.31f, 0.0f, 1.45f), true, false);
        combine.AddWheelAnchor(new Vector3(1.16f, 0.0f, -1.0f), false, true);
        combine.AddWheelAnchor(new Vector3(-1.31f, 0.0f, 1.45f), true, false);
        combine.AddWheelAnchor(new Vector3(-1.16f, 0.0f, -1.0f), false, true);
        parts.Add(combine);

        // sphere
        var sphere = new BodyPart("Sphere", "res://assets/parts/body/sphere-body.tscn")
        {
            Description = "Behold the smoothness.",
            Mass = 400.0f,
            SoundPart = new Engine1SoundPart(),
        };
        sphere.AddWheelAnchor(new Vector3(1.8f, -1.5f, 1.8f), false, true);
        sphere.AddWheelAnchor(new Vector3(1.8f, -1.5f, -1.8f), true, false);
        sphere.AddWheelAnchor(new Vector3(-1.8f, -1.5f, 1.8f), false, true);
        sphere.AddWheelAnchor(new Vector3(-1.8f, -1.5f, -1.8f), true, false);
        parts.Add(sphere);

        // box
        var box = new BodyPart("Box", "res://assets/parts/body/box-body.tscn")
        {
            Description = "A box. For boxing?",
            Mass = 200.0f,
            SoundPart = new Engine1SoundPart(),
        };
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, 3.5f), false, true);
        box.AddWheelAnchor(new Vector3(1.8f, -0.9f, -3.5f), true, false);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, 3.5f), false, true);
        box.AddWheelAnchor(new Vector3(-1.8f, -0.9f, -3.5f), true, false);

        parts.Add(box);

        return parts;
    }
}
