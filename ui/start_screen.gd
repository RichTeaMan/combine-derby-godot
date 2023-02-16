extends Control

func _on_Button_pressed():
	Global.create_game(2, "", "")
	
	queue_free()
