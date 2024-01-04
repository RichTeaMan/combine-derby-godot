class_name VehiclePart

enum PART_TYPE { BODY, WHEELS, ENGINE, ATTACHMENTS }

var name: String
var part_type: PART_TYPE
var scene_asset: Resource

func _init(name: String, scene_path: String):
	self.name = name
	self.scene_asset = load(scene_path)

func instantiate_scene():
	return scene_asset.instantiate()

static func parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	parts.append_array(BodyPart.body_parts_init())
	parts.append_array(WheelPart.wheel_parts_init())
	
	return parts
