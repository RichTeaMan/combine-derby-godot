extends Spatial

var exploded = false

func barrel_body_entered(body: Node):
	print("combine?")
	if not exploded and Global.is_in_vehicle_group(body):
		print("explosion!")
		Global.add_points(body.player_id, 20, "barrel booms")
		exploded = true

		# start explosion particle emitter
		$"%explosion_particles".one_shot = true
		$"%explosion_particles".emitting = true
		$"%explosion_sound".playing = true
		$"%model".visible = false
		$"%smoke_particles".emitting = false

func _process(_delta):
	if exploded and not $"%explosion_particles".emitting and not $"%explosion_sound".playing:
		# delete barrel
		queue_free()
