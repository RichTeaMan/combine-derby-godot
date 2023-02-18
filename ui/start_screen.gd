extends Control

func _on_one_player_pressed():
	start_game(1)
	
func _on_two_player_pressed():
	start_game(2)

func _input(_ev):
	if Input.is_action_just_pressed("ui_accept"):
		start_game(1)

func start_game(player_count):
	Global.create_game(player_count, "", "")	
	queue_free()

func _on_sound_enabled_chb_toggled(button_pressed):
	Global.set_mute(!button_pressed)
