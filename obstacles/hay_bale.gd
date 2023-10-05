extends Node3D

var bale_explode_template

func _ready():
	bale_explode_template = preload("res://obstacles/hay_bale_explode.tscn")

func hay_body_entered(body: Node):
	if Global.is_in_vehicle_group(body):
		Global.do_vehicle_pickup(body.player_id, "Hay bales", 1)

		# create hay bale particle emitter
		var explode_instance = bale_explode_template.instantiate()
		var current_transform = $RigidBody3D.global_transform
		explode_instance.set_transform(current_transform)
		get_tree().get_root().add_child(explode_instance)

		# delete hay bale
		queue_free()
