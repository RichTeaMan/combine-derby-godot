extends Spatial

signal points(point_increment)

signal speed(speed_ms, vehicle_id)

func add_points(points):
	emit_signal("points", points)

func update_speed(speed_ms, vehicle_id):
	emit_signal("speed", speed_ms, vehicle_id)

func is_in_vehicle_group(node: Node):
	return node.is_in_group("vehicle")
