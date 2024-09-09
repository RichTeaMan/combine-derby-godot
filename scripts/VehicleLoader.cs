using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class VehicleLoader
{
    private Dictionary<string, VehiclePart> _vehicleParts = null;

    public Dictionary<string, VehiclePart> VehicleParts
    {
        get
        {
            if (_vehicleParts == null)
            {
                var partsList = VehiclePart.PartsInit();
                _vehicleParts = partsList.ToDictionary(k => k.Name, v => v);
            }
            return _vehicleParts;
        }
    }


    const string FILE_EXTENSION = ".json";

    public string[] LoadUserVehicleNames()
    {
        GD.Print($"Peeking user vehicles files.");
        DirAccess.MakeDirRecursiveAbsolute("user://vehicles");
        var files = DirAccess.GetFilesAt("user://vehicles");
        return files
            .Where(f => f.EndsWith(FILE_EXTENSION))
            .Select(f => f[..^FILE_EXTENSION.Length]).ToArray();
    }

    public CdVehicle LoadUserVehicleFromModel(CdVehicle.CdVehicleModel model)
    {
        if (model == null) {
            GD.Print("Cannot load vehicle, model is null.");
            return null;
        }
        GD.Print($"Loading vehicle from model with name: {model.Name}.");
        var vehicle = new CdVehicle()
        {
            Model = model,
        };
        vehicle.RebuildFromParts(VehicleParts);
        return vehicle;
    }

    public CdVehicle.CdVehicleModel LoadUserVehicleModelFromFilepath(string filepath)
    {
        using var saveFile = FileAccess.Open(filepath, FileAccess.ModeFlags.Read);
        if (saveFile == null) {
            return null;
        }
        string json = saveFile.GetAsText();
        var options = new JsonSerializerOptions { IncludeFields = true };
        var model = JsonSerializer.Deserialize<CdVehicle.CdVehicleModel>(json, options);
        GD.Print("Load model complete");
        return model;
    }

    public CdVehicle.CdVehicleModel LoadUserVehicleModelFromName(string name)
    {
        GD.Print($"Loading vehicle from name: {name}.");
        string filepath = $"user://vehicles/{name}.json";
        return LoadUserVehicleModelFromFilepath(filepath);
    }
}