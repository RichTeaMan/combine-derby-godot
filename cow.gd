extends Spatial

func cow_body_entered(body:Node):
	print("cow collision")
	$sound.play()
