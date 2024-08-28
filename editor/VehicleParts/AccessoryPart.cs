using System.Collections.Generic;

public class AccessoryPart : VehiclePart
{

    public AccessoryPart(string name, string scenePath) : base(name, scenePath) {
        PartType = PartType.Accessory;
    }


    public static List<AccessoryPart> AccessoryPartsInit()
    {
        var parts = new List<AccessoryPart>();

        // basic wheel
        var basicWheel = new AccessoryPart("Basic Wheel accs", "res://assets/parts/wheels/basic-wheel.tscn")
        {
            Description = "It's round, and it's low effor.",
            Mass = 2.0f,
        };
        parts.Add(basicWheel);

        return parts;
    }
}
