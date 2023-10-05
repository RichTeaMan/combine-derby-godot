extends Node3D

var exploded = false

func barrel_body_entered(body: Node):
	if not exploded and Global.is_in_vehicle_group(body):
		Global.do_vehicle_pickup(body.player_id, "Barrel booms", 1)
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
