extends VehicleBody3D

const UPSIDE_DOWN_ANGLE = PI * 0.75
var UPSIDE_DOWN_FRAMES_LIMIT = Engine.physics_ticks_per_second * 2

@export var player_id = 1

var max_rpm = 500.0
var max_torque = 2000
var idle_sound_db = 0.0
var accelerating_sound_db = 6.0

var upside_down_frames = 0

var steering_left_input
var steering_right_input
var forward_input
var back_input

## Changes rotation so the combine is on its wheels.
func roll_to_wheels():
	rotation.z = 0.0

func _ready():
	steering_left_input = "player%d_left" % player_id
	steering_right_input = "player%d_right" % player_id
	forward_input = "player%d_forward" % player_id
	back_input = "player%d_back" % player_id
	$sound.play()

func _physics_process(delta):
	steering = lerp(steering, Input.get_axis(steering_right_input,steering_left_input) * -0.4, 2 * delta)
	var acceleration = Input.get_axis(back_input, forward_input)

	var left_rpm = abs($back_left_wheel.get_rpm())
	$back_left_wheel.engine_force = acceleration * max_torque * (1 - left_rpm / max_rpm)

	var right_rpm = abs($back_right_wheel.get_rpm())
	$back_right_wheel.engine_force = acceleration * max_torque * (1 - right_rpm / max_rpm)

	Global.update_speed(player_id, get_linear_velocity().length())

	if acceleration != 0:
		$sound.volume_db = accelerating_sound_db
	else:
		$sound.volume_db = idle_sound_db

	if rotation.z > UPSIDE_DOWN_ANGLE || rotation.z < -UPSIDE_DOWN_ANGLE:
		upside_down_frames += 1
		if upside_down_frames >= UPSIDE_DOWN_FRAMES_LIMIT:
			roll_to_wheels()
	else:
		upside_down_frames = 0
	if Input.is_action_just_pressed("reset"):
		roll_to_wheels()

func is_reversing():
	return $back_left_wheel.get_rpm() < 0 && $back_right_wheel.get_rpm() < 0

func _integrate_forces(state: PhysicsDirectBodyState3D) -> void:
	if state.get_contact_count() == 0:
		return
	var collision_force = Vector3.ZERO
	for i in range(state.get_contact_count()):
		collision_force += state.get_contact_impulse(i) * state.get_contact_local_normal(i)
	if collision_force != Vector3.ZERO:
		
		#print("combine collsion force %s" % collision_force.length_squared())
		pass
	if collision_force.length_squared() > 30000.0:
		$crash_sounds.play_big_sound()
	elif collision_force.length_squared() > 100.0:
		$crash_sounds.play_small_sound()

func _on_vehicle_body_shape_entered(_body_rid, _body, _body_shape_index, _local_shape_index):
	# integrate forces seem to miss some collision (usually static bodies, but not always)
	# this seems to find the rest of them. big crashes are assumed
	#print("combine body shape entered")
	$crash_sounds.play_big_sound()

#func setup_camera_viewport():
#	$camera_pivot/camera.
