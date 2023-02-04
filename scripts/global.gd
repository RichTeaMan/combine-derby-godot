extends Spatial

signal points(point_increment)

func add_points(points):
	emit_signal("points", points)
