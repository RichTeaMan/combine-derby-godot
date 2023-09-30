extends Node3D

func cow_body_entered(body: Node):
	if !$"%sound".is_playing() && Global.is_in_vehicle_group(body):
		$"%sound".play()
		Global.add_points(body.player_id, 5, "Bull bounces")
