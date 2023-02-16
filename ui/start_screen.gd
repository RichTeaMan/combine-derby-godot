extends Control

func _on_Button_pressed():
	start_game()

func _input(_ev):
	if Input.is_action_just_pressed("ui_accept"):
		start_game()
	
	
func start_game():
	Global.create_game(2, "", "")	
	queue_free()
