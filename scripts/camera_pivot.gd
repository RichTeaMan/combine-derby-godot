extends Node3D

@onready var direction = get_parent_node_3d().rotation
@export_range(1,10,0.1) var smooth_speed: float = 7.0

func _physics_process(delta):
	var parent: VehicleBody3D = get_parent_node_3d()
	var vehicle_basis = parent.global_transform.basis
	var current_velocity: Vector3 = get_parent().get_linear_velocity()
	current_velocity.y = 0
	if current_velocity.length_squared() > 1.0:
		direction = lerp(direction, -current_velocity.normalized(), smooth_speed*delta)
		# math hack to prevent camera from looking at the front of the vehicle when reversing
		# there must be a better way but I don't know what it is.
		# mostly works, but camera does shudder with crashes or other sudden movements
		global_transform.basis = get_rotation_from_direction(direction, vehicle_basis.tdotz(-direction.normalized()) < 0.0)

func get_rotation_from_direction(look_direction: Vector3, is_reversing: bool) -> Basis:
	look_direction = look_direction.normalized()
	if is_reversing:
		look_direction = -look_direction
	var x_axis = look_direction.cross(Vector3.UP)
	return Basis(x_axis, Vector3.UP, -look_direction)
