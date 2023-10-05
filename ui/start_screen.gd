extends Control

func fetch_player_count() -> int:
	var player_select: OptionButton = %"player_select"
	return player_select.get_selected_id()

func on_arena_game():
	start_game(Global.MODE_POINTS, Global.ARENA_CRASH)
	
func on_harvest_game():
	start_game(Global.MODE_HARVEST, Global.ARENA_FARM)

func _input(_ev):
	if Input.is_action_just_pressed("ui_accept"):
		start_game(Global.MODE_POINTS, Global.ARENA_CRASH)

func start_game(game_mode: String, game_type: String):
	var player_count = fetch_player_count()
	var callback = Callable(self, "create_game")
	Transitions.fade_func(callback, [player_count, game_mode, game_type])

func create_game(player_count: int, game_mode: String, game_type: String):
	Global.create_game(player_count, game_mode, game_type)
	Transitions.fade_back()
	queue_free()

func _on_sound_enabled_chb_toggled(button_pressed):
	Global.set_mute(!button_pressed)

func _on_github_link_button_pressed():
	OS.shell_open("https://github.com/RichTeaMan/combine-derby-godot")
