class_name VehiclePart

var name: String
var part_type: Enums.PART_TYPE
var scene_asset: Resource

func _init(name: String, scene_path: String):
	self.name = name
	self.scene_asset = load(scene_path)

func instantiate_scene():
	return scene_asset.instantiate()
	
func part_type_str():
	return Enums.PART_TYPE.keys()[part_type]

static func parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	parts.append_array(BodyPart.body_parts_init())
	parts.append_array(WheelPart.wheel_parts_init())
	
	print("Loaded parts:")
	for p in parts:
		print("%s - %s" % [p.part_type_str(), p.name])
	
	return parts
