extends Spatial

signal points(point_increment)

signal speed(speed_ms, vehicle_id)

var pause_menu

var camera_reverses: bool = true

var min_sound = -60.0
var max_sound = 6.0

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

## Sets master volume. Volume should between 0.0 and 1.0.
func set_master_volume(volume):
	var resolved_db= resolve_volume_fraction_to_db(volume)
	print("Master volume set to %s, resolved db set to %s" % [volume, resolved_db])
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Master"), resolved_db)

func get_master_volume():
	var db = AudioServer.get_bus_volume_db(AudioServer.get_bus_index("Master"))
	return resolve_db_volume_fraction(db)

## Sets sfx volume. Volume should between 0.0 and 1.0.
func set_sfx_volume(volume):
	var resolved_db= resolve_volume_fraction_to_db(volume)
	print("SFX volume set to %s, resolved db set to %s" % [volume, resolved_db])
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("sfx"), resolved_db)

func get_sfx_volume():
	var db = AudioServer.get_bus_volume_db(AudioServer.get_bus_index("sfx"))
	return resolve_db_volume_fraction(db)

func resolve_volume_fraction_to_db(volume):
	if volume > 1.0:
		volume = 1.0
	elif volume < 0.0:
		volume = 0.0
	var sound_range = max_sound - min_sound
	var resolved_sound = (volume * sound_range) + min_sound
	return resolved_sound

func resolve_db_volume_fraction(db):
	var adjusted_db = db - min_sound
	var sound_range = max_sound - min_sound
	return adjusted_db / sound_range
