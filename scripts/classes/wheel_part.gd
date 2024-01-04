class_name WheelPart extends VehiclePart

func _init(name: String, scene_path: String):
	super(name, scene_path)
	self.part_type = Enums.PART_TYPE.WHEELS

static func wheel_parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	
	# basic wheel
	var basic_wheel = WheelPart.new("Basic Wheel", "res://assets/parts/body/basic-wheel.tscn")
	parts.append(basic_wheel)
	
	return parts
