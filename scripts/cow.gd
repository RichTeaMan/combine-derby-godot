extends Spatial

func cow_body_entered(body:Node):
	if !$sound.is_playing() && Global.is_in_vehicle_group(body):
		$sound.play()
