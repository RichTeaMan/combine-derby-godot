extends VehicleBody

var max_rpm = 500.0
var max_torque = 2000
var idle_sound_db = 0.0
var accelerating_sound_db = 6.0

func _ready():
	$sound.play()

func _physics_process(delta):
	steering = lerp(steering, Input.get_axis("right","left") * -0.4, 5 * delta)
	var acceleration = Input.get_axis("back","forward")

	var left_rpm = abs($back_left_wheel.get_rpm())
	$back_left_wheel.engine_force = acceleration * max_torque * (1 - left_rpm / max_rpm)

	var right_rpm = abs($back_right_wheel.get_rpm())
	$back_right_wheel.engine_force = acceleration * max_torque * (1 - right_rpm / max_rpm)

	Global.update_speed(get_linear_velocity().length(), 1)

	if acceleration != 0:
		$sound.unit_db = accelerating_sound_db
	else:
		$sound.unit_db = idle_sound_db

func _input(_ev):
	if Input.is_action_just_pressed("reset"):
		rotation = Vector3.UP

func is_reversing():
	return $back_left_wheel.get_rpm() < 0 && $back_right_wheel.get_rpm() < 0


func _integrate_forces(state: PhysicsDirectBodyState) -> void:
	if state.get_contact_count() == 0:
		return
	var collision_force = Vector3.ZERO
	for i in range(state.get_contact_count()):
		collision_force += state.get_contact_impulse(i) * state.get_contact_local_normal(i)
	if collision_force != Vector3.ZERO:
		print("collsion force %s" % collision_force.length_squared())
	if collision_force.length_squared() > 30000.0:
		$crash_sounds.play_big_sound()
	elif collision_force.length_squared() > 100.0:
		$crash_sounds.play_small_sound()

func _on_vehicle_body_shape_entered(body_rid, body, body_shape_index, local_shape_index):
	# integrate forces seem to miss some collision (usually static bodies, but not always)
	# this seems to find the rest of them. big crashes are assumed
	$crash_sounds.play_big_sound()
