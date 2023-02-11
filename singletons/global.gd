extends Spatial

signal points(point_increment)

signal speed(speed_ms, vehicle_id)

var pause_menu

var camera_reverses: bool = true

func _ready():
	var pause_menu_template = preload("res://ui/pause_menu.tscn")
	pause_menu = pause_menu_template.instance()

func add_points(points):
	emit_signal("points", points)

func update_speed(speed_ms, vehicle_id):
	emit_signal("speed", speed_ms, vehicle_id)

func is_in_vehicle_group(node: Node):
	return node.is_in_group("vehicle")

func _input(_event):
	if Input.is_action_just_pressed("menu"):
		if get_tree().paused:
			get_tree().paused = false
			get_tree().get_root().remove_child(pause_menu)
		else:
			get_tree().paused = true
			get_tree().get_root().add_child(pause_menu)
