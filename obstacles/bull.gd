extends Node3D

func cow_body_entered(body: Node):
	if !$"%sound".is_playing() && Global.is_in_vehicle_group(body):
		$"%sound".play()
		Global.do_vehicle_pickup(body.player_id, "Bull bounces", 1)
