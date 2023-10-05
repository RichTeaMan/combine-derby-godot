extends Node3D

const ARENA_CRASH = "crash"
const ARENA_FARM = "farm"

const MODE_FREEPLAY = "freeplay"
const MODE_POINTS = "points"
const MODE_HARVEST = "harvest"

signal vehicle_pickup(player_id: int, category, quantity: int)

signal vehicle_body_shape_entered(player_id: int, body: Node3D)

signal game_info_ui(player_id: int, message: String)

signal speed(player_id: int, speed_ms: float)

signal player_added(player_id: int, node: Node3D)

signal player_ui(player_id: int, node: Node3D)

signal gfx_settings_updated()

var pause_menu

var current_game_scene: Node
var current_player_count = 1

var camera_reverses: bool = true

var min_sound = -60.0
var max_sound = 6.0

var current_playlist: Array
var current_playlist_index: int

var gfx_scaling = 0.5

func _ready() -> void:
	if is_web():
		gfx_scaling = 0.4
	var pause_menu_template = preload("res://ui/pause_menu.tscn")
	pause_menu = pause_menu_template.instantiate()

func add_player(player_id: int, node: Node) -> void:
	emit_signal("player_added", player_id, node)

func do_vehicle_pickup(player_id: int, category: String, quantity: int) -> void:
	emit_signal("vehicle_pickup", player_id, category, quantity)

func do_vehicle_body_shape_entered(player_id: int, body: Node3D) -> void:
	emit_signal("vehicle_body_shape_entered", player_id, body)

func set_game_info_ui(player_id: int, message: String) -> void:
	emit_signal("game_info_ui", player_id, message)

func update_speed(player_id: int, speed_ms: float) -> void:
	emit_signal("speed", player_id, speed_ms)

func add_player_ui(player_id: int, node: Node) -> void:
	emit_signal("player_ui", player_id, node)

func do_restart_game() -> void:
	current_game_scene.queue_free()
	call_deferred("create_game", current_player_count, "", "")

func do_gfx_settings_updated() -> void:
	print("GFX scaling set to %s." % gfx_scaling)
	emit_signal("gfx_settings_updated")

func is_in_vehicle_group(node: Node):
	return node.is_in_group("vehicle")

func get_screen_width():
	return get_viewport().size.x

func get_screen_height():
	return get_viewport().size.y

func _input(_event) -> void:
	if Input.is_action_just_pressed("menu"):
		if get_tree().paused:
			close_pause()
		else:
			open_pause()

func open_pause() -> void:
	get_tree().paused = true
	get_tree().get_root().add_child(pause_menu)

func close_pause() -> void:
	get_tree().paused = false
	get_tree().get_root().remove_child(pause_menu)

func is_web():
	return OS.get_name() == "HTML5"
 
func set_mute(mute) -> void:
	print("Master bus muted: %s" % [mute])
	AudioServer.set_bus_mute(AudioServer.get_bus_index("Master"), mute)

func get_mute() -> bool:
	return AudioServer.is_bus_mute(AudioServer.get_bus_index("Master"))

## Sets master volume. Volume should between 0.0 and 1.0.
func set_master_volume(volume) -> void:
	var resolved_db= resolve_volume_fraction_to_db(volume)
	print("Master volume set to %s, resolved db set to %s" % [volume, resolved_db])
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Master"), resolved_db)

func get_master_volume():
	var db = AudioServer.get_bus_volume_db(AudioServer.get_bus_index("Master"))
	return resolve_db_volume_fraction(db)
	
## Sets music volume. Volume should between 0.0 and 1.0.
func set_music_volume(volume) -> void:
	var resolved_db= resolve_volume_fraction_to_db(volume)
	print("Music volume set to %s, resolved db set to %s" % [volume, resolved_db])
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("music"), resolved_db)

func get_music_volume():
	var db = AudioServer.get_bus_volume_db(AudioServer.get_bus_index("music"))
	return resolve_db_volume_fraction(db)

## Sets sfx volume. Volume should between 0.0 and 1.0.
func set_sfx_volume(volume) -> void:
	var resolved_db= resolve_volume_fraction_to_db(volume)
	print("SFX volume set to %s, resolved db set to %s" % [volume, resolved_db])
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("sfx"), resolved_db)

func get_sfx_volume():
	var db = AudioServer.get_bus_volume_db(AudioServer.get_bus_index("sfx"))
	return resolve_db_volume_fraction(db)

func resolve_volume_fraction_to_db(volume):
	if volume > 1.0:
		volume = 1.0
	elif volume < 0.0:
		volume = 0.0
	var sound_range = max_sound - min_sound
	var resolved_sound = (volume * sound_range) + min_sound
	return resolved_sound

func resolve_db_volume_fraction(db):
	var adjusted_db = db - min_sound
	var sound_range = max_sound - min_sound
	var volume = adjusted_db / sound_range
	if volume > 1.0:
		volume = 1.0
	elif volume < 0.0:
		volume = 0.0
	return volume

func play_music(name: String) -> void:
	var file = "res://assets/music/%s" % name
	if ResourceLoader.exists(file):
		print("Playing %s" % name)
		var music = load(file) 
		music.set_loop(false)
		$music_player.stream = music
		$music_player.play()
	else:
		print("Unable to play %s, file not found" % name)
	
func set_playlist(playlist: Array, random_start = false) -> void:
	if playlist != null && playlist.size() > 0:
		current_playlist = playlist
		current_playlist_index = 0
		if random_start:
			current_playlist_index = randi() % playlist.size()
		play_music(current_playlist[current_playlist_index])

func _on_music_player_finished() -> void:
	if current_playlist == null || current_playlist.size() == 0:
		return
	current_playlist_index += 1
	if current_playlist_index == current_playlist.size():
		current_playlist_index = 0
	play_music(current_playlist[current_playlist_index])

func create_game(player_count: int, game_mode: String, arena_name: String) -> void:
	print("Create game. Players: %s" % [player_count])

	for player in get_tree().get_nodes_in_group("player"):
		player.remove_from_group("player")

	var player_container
	if player_count == 1:
		player_container = preload("res://player_frames/one_player.tscn")
	elif player_count == 2:
		player_container = preload("res://player_frames/two_player.tscn")
	else:
		print("Unsupported number of players '%s'" % [player_count])
		get_tree().quit()
	var instance = player_container.instantiate()
	print("Adding game instance...")
	get_parent().add_child(instance)

	print("Setting up game mode %s", [game_mode])
	var game_type
	if game_mode == MODE_POINTS:
		game_type = preload("res://game_rules/points.tscn")
	elif game_mode == MODE_HARVEST:
		game_type = preload("res://game_rules/harvest.tscn")
	else:
		print("Unknown game mode' %s'" % [game_mode])		
		get_tree().quit()
	var game_instance = game_type.instantiate()
	game_instance.player_count = player_count
	
	var arena
	if arena_name == ARENA_CRASH:
		arena = preload("res://arenas/crash.tscn")
	elif arena_name == ARENA_FARM:
		arena = preload("res://arenas/farm.tscn")
	else:
		print("Unknown arena '%s'" % [arena_name])
		get_tree().quit()
	
	game_instance.add_child(arena.instantiate())

	var combine_template = preload("res://vehicles/combine.tscn")
	for i in player_count:
		var player_id = i + 1
		var combine_instance = combine_template.instantiate()
		combine_instance.player_id = player_id
		combine_instance.add_to_group("player", true)
		add_player(combine_instance.player_id, combine_instance)

	print("Combines added")
	instance.add_child(game_instance)
	current_game_scene = instance
	current_player_count = player_count
