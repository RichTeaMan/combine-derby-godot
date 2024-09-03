using System.Collections.Generic;

public class AccessoryPart : VehiclePart
{

    public AccessoryPart(string name, string scenePath) : base(name, scenePath) {
        PartType = PartType.Accessory;
    }


    public static List<AccessoryPart> AccessoryPartsInit()
    {
        var parts = new List<AccessoryPart>();
        
        var flag = new AccessoryPart("Flag", "res://assets/parts/accessories/flag.tscn") {
            Description = "Wave your flag.",
            Mass = 2.0f
        };
        parts.Add(flag);

        return parts;
    }
}
