extends Node3D

@export var player_count: int

var current_points = 0
var countdown_length_seconds = 60
var game_active = true
var player_points: Array

func set_game_info(player_id: int):
	Global.set_game_info_ui(player_id, "    Harvest time!!!")

func _ready():
	refresh_timer()
	var _a = Global.connect("points", Callable(self, "_on_points"))
	for player_id in player_count:
		set_game_info(player_id + 1)

func _enter_tree():
	player_points = []
	for i in player_count + 1:
		var p = {}
		p["points"] = 0
		player_points.append(p)

func _on_points(player_id: int, points: int, category: String) -> void:
	if player_id <= player_count:
		if not category in player_points[player_id]:
			player_points[player_id][category] = 0
			print("reset")
		player_points[player_id][category] += 1
		player_points[player_id]["points"] += points

func _on_Timer_timeout():
	return
	#if countdown_length_seconds <= 0:
	#	game_active = false
	#	$Timer.stop()
	#	var game_over_scene = preload("res://ui/game_over.tscn")

	#	for player_index in player_count:
	#		var player_id = player_index + 1
	#		var game_over_instance = game_over_scene.instantiate()
	#		game_over_instance.player_id = player_id
	#		game_over_instance.points = player_points[player_id]
	#		Global.add_player_ui(player_id, game_over_instance)
	#	return
	#countdown_length_seconds += -1
	#refresh_timer()

func refresh_timer():
	var minutes_remaining = countdown_length_seconds / 60
	var seconds_remaining = countdown_length_seconds % 60
	$"%time_label".text = "[center]%s:%02d[/center]" % [ minutes_remaining, seconds_remaining ]
