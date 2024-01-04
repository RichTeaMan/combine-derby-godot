class_name BodyPart extends VehiclePart

func _init(name: String, scene_path: String):
	super(name, scene_path)
	part_type = PART_TYPE.BODY

static func body_parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	
	# silo
	var silo = BodyPart.new("Silo", "res://assets/parts/body/silo-body.tscn")
	parts.append(silo)
	
	return parts
