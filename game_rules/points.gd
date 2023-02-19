extends Spatial

export var player_count: int

var current_points = 0
var countdown_length_seconds = 60
var game_active = true
var player_points: Array

func _ready():
	refresh_timer()
	var _a = Global.connect("points", self, "_on_points")

func _enter_tree():
	var arena = preload("res://arena.tscn")
	player_points = []
	for i in player_count + 1:
		var p = {}
		p["points"] = 0
		player_points.append(p)
	add_child(arena.instance())

func _on_points(player_id: int, points: int, category: String) -> void:
	if player_id <= player_count:
		if not category in player_points[player_id]:
			player_points[player_id][category] = 0
			print("reset")
		player_points[player_id][category] += 1
		player_points[player_id]["points"] += points

func _on_Timer_timeout():
	if countdown_length_seconds <= 0:
		game_active = false
		$Timer.stop()
		var game_over_scene = preload("res://ui/game_over.tscn")

		for player_index in player_count:
			var player_id = player_index + 1
			var game_over_instance = game_over_scene.instance()
			game_over_instance.player_id = player_id
			game_over_instance.points = player_points[player_id]
			Global.add_player_ui(player_id, game_over_instance)
		return
	countdown_length_seconds += -1
	refresh_timer()

func refresh_timer():
	var minutes_remaining = countdown_length_seconds / 60
	var seconds_remaining = countdown_length_seconds % 60
	$"%time_label".bbcode_text = "[center]%s:%02d[/center]" % [ minutes_remaining, seconds_remaining ]
