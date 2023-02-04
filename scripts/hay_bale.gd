extends Spatial

func hay_body_entered(body:Node):
	if body.is_in_group("vehicle"):
		Global.add_points(10)
		queue_free()
	#$sound.play()
