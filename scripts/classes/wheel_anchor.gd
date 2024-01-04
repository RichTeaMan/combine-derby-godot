class_name WheelAnchor

var attachment_point: Vector3
var is_traction: bool
var is_steering: bool

var base_rotation: Vector3 = Vector3.ZERO

func _init(attachment_point: Vector3, is_traction: bool, is_steering: bool):
	self.attachment_point = attachment_point
	self.is_traction = is_traction
	self.is_steering = is_steering
	
	# guess rotation
	if attachment_point.x < 0:
		base_rotation = Vector3(0, PI, 0)
