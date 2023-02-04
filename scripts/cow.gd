extends Spatial

func cow_body_entered(_body:Node):
	print("cow collision")
	$sound.play()
