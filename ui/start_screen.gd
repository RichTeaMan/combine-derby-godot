extends Control

func _on_one_player_pressed():
	start_game(1)
	
func _on_two_player_pressed():
	start_game(2)

func _input(_ev):
	if Input.is_action_just_pressed("ui_accept"):
		start_game(1)

func start_game(player_count):
	var callback = funcref(self, "create_game")
	Transitions.fade_func(callback, [player_count])

func create_game(player_count: int):
	Global.create_game(player_count, "","")
	Transitions.fade_back()
	queue_free()

func _on_sound_enabled_chb_toggled(button_pressed):
	Global.set_mute(!button_pressed)

func _on_github_link_button_pressed():
	OS.shell_open("https://github.com/RichTeaMan/combine-derby-godot")
