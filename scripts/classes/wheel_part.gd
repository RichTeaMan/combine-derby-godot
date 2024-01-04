class_name WheelPart extends VehiclePart

func _init(name: String, scene_path: String):
	super(name, scene_path)
	part_type = PART_TYPE.WHEELS

static func wheel_parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	
	# basic wheel
	var basic_wheel = BodyPart.new("Basic Wheel", "res://assets/parts/body/basic-wheel.tscn")
	parts.append(basic_wheel)
	
	return parts
