extends Spatial

func _input(_event):
	if Input.is_action_just_pressed("exit"):
		get_tree().quit()
