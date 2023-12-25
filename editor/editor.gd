extends Node3D

var rotating = false
var rotation_constant = 0.5
var zoom_constant = 0.2
var prev_mouse_position
var next_mouse_position

func _process(delta):
	if (Input.is_action_just_pressed("rotate")):
		rotating = true
		prev_mouse_position = get_viewport().get_mouse_position()
	if (Input.is_action_just_released("rotate")):
		rotating = false
	
	if (rotating):
		next_mouse_position = get_viewport().get_mouse_position()
		%gimbal.rotate_y((next_mouse_position.x - prev_mouse_position.x) * rotation_constant * delta)
		%gimbal.rotate_x((next_mouse_position.y - prev_mouse_position.y) * rotation_constant * delta)
		prev_mouse_position = next_mouse_position
	
	if (Input.is_action_just_pressed("zoom_in")):
		%camera.position.z -= zoom_constant
	
	if (Input.is_action_just_pressed("zoom_out")):
		%camera.position.z += zoom_constant
	
