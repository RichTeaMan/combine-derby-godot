extends Spatial

var exploded = false

func barrel_body_entered(body: Node):
	print("combine?")
	if Global.is_in_vehicle_group(body):
		print("explosion!")
		Global.add_points(body.player_id, 20, "barrel booms")
		exploded = true

		# start explosion particle emitter
		$"%explosion_particles".one_shot = true
		$"%explosion_particles".emitting = true
		

func _process(_delta):
	if exploded and not $"%explosion_particles".emitting:
		# delete barrel
		queue_free()
