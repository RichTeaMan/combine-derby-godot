class_name BodyPart extends VehiclePart

var wheel_anchors: Array[WheelAnchor] = []

func _init(name: String, scene_path: String):
	super(name, scene_path)
	self.part_type = Enums.PART_TYPE.BODY

func add_wheel_anchor(anchor_point: Vector3, is_traction: bool, is_steering: bool):
	var anchor = WheelAnchor.new(anchor_point, is_traction, is_steering)
	wheel_anchors.append(anchor)

static func body_parts_init() -> Array[VehiclePart]:
	var parts: Array[VehiclePart] = []
	
	# silo
	var silo = BodyPart.new("Silo", "res://assets/parts/body/silo-body.tscn")
	silo.add_wheel_anchor(Vector3(1.5,-0.8,3), true, true)
	silo.add_wheel_anchor(Vector3(1.5,-0.8,-3), true, true)
	silo.add_wheel_anchor(Vector3(-1.5,-0.8,3), true, true)
	silo.add_wheel_anchor(Vector3(-1.5,-0.8,-3), true, true)
	parts.append(silo)
	
	return parts
