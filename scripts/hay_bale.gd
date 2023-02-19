extends Spatial

var bale_explode_template

func _ready():
	bale_explode_template = preload("res://hay_bale_explode.tscn")

func hay_body_entered(body: Node):
	if Global.is_in_vehicle_group(body):
		Global.add_points(body.player_id, 10, "hay bales")

		# create hay bale particle emitter
		var explode_instance = bale_explode_template.instance()
		var current_transform = $RigidBody.global_transform
		explode_instance.set_transform(current_transform)
		get_tree().get_root().add_child(explode_instance)

		# delete hay bale
		queue_free()
