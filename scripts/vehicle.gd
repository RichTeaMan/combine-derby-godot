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

var traction_wheels: Array[VehicleWheel3D] = []
var steering_wheels: Array[VehicleWheel3D] = []

## Changes rotation so the combine is on its wheels.
func roll_to_wheels():
	rotation.z = 0.0

func rebuild_wheels():
	traction_wheels = []
	steering_wheels = []
	
	var child_nodes = Utils.children_recursive(self)
	for node in child_nodes:
		print(node)
		if node is VehicleWheel3D:
			var wheel  = node as VehicleWheel3D
			if wheel.use_as_traction:
				traction_wheels.append(wheel)
			if wheel.use_as_steering:
				steering_wheels.append(wheel)
	print("Found %s steering wheels, %s traction wheels." % [steering_wheels.size(), traction_wheels.size()])

func _ready():
	steering_left_input = "player%d_left" % player_id
	steering_right_input = "player%d_right" % player_id
	forward_input = "player%d_forward" % player_id
	back_input = "player%d_back" % player_id
	#$sound.play()

func _physics_process(delta):
	steering = lerp(steering, Input.get_axis(steering_right_input,steering_left_input) * -0.4, 2 * delta)
	var acceleration = Input.get_axis(back_input, forward_input)

	for wheel in traction_wheels:
		if !is_instance_valid(wheel) || wheel.is_queued_for_deletion():
			continue
		var rpm = abs(wheel.get_rpm())
		wheel.engine_force = acceleration * max_torque * (1 - rpm / max_rpm)

	Global.update_speed(player_id, basis.tdotz(get_linear_velocity()))

	#if acceleration != 0:
	#	$sound.volume_db = accelerating_sound_db
	#else:
	#	$sound.volume_db = idle_sound_db

	if rotation.z > UPSIDE_DOWN_ANGLE || rotation.z < -UPSIDE_DOWN_ANGLE:
		upside_down_frames += 1
		if upside_down_frames >= UPSIDE_DOWN_FRAMES_LIMIT:
			roll_to_wheels()
	else:
		upside_down_frames = 0
	if Input.is_action_just_pressed("reset"):
		roll_to_wheels()

func is_reversing():
	# speed that will go negative if reversing
	var speed = basis.tdotz(get_linear_velocity())
	return speed < 0.0

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

func _on_vehicle_body_shape_entered(_body_rid: RID, body: Node, _body_shape_index: int, _local_shape_index: int):
	# integrate forces seem to miss some collision (usually static bodies, but not always)
	# this seems to find the rest of them. big crashes are assumed
	$crash_sounds.play_big_sound()
	Global.do_vehicle_body_shape_entered(player_id, body)

