extends VehicleBody

var max_rpm = 500.0
var max_torque = 2000
var idle_sound_db = 0.0
var accelerating_sound_db = 6.0

func _ready():
	$sound.play()

func _physics_process(delta):
	steering = lerp(steering, Input.get_axis("right","left") * 0.4, 5 * delta)
	var acceleration = Input.get_axis("back","forward")
	
	var left_rpm = $back_left_wheel.get_rpm()
	$back_left_wheel.engine_force = acceleration * max_torque * (1 - left_rpm / max_rpm)

	var right_rpm = $back_right_wheel.get_rpm()
	$back_right_wheel.engine_force = acceleration * max_torque * (1 - right_rpm / max_rpm)

	if acceleration != 0:
		$sound.unit_db = accelerating_sound_db
	else:
		$sound.unit_db = idle_sound_db

func _input(_ev):
	if Input.is_action_just_pressed("reset"):
		rotation = Vector3.UP
