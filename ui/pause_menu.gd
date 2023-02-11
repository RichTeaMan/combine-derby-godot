extends MarginContainer

func _input(_event):
	if Input.is_action_just_pressed("menu_quit"):
		print("Quit key pressed")
		get_tree().quit()
