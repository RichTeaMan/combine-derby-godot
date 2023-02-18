extends Spatial

export var player_count: int

signal gameover_update(gameover_dict)

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
	player_points.resize(player_count + 1)
	player_points.fill(0)
	add_child(arena.instance())

func _on_points(player_id: int, points: int) -> void:
	if player_id <= player_count:
		player_points[player_id] += points

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

		emit_signal("gameover_update", {"score": current_points})
		return
	countdown_length_seconds += -1
	refresh_timer()

func refresh_timer():
	var minutes_remaining = countdown_length_seconds / 60
	var seconds_remaining = countdown_length_seconds % 60
	$"%time_label".bbcode_text = "[center]%s:%02d[/center]" % [ minutes_remaining, seconds_remaining ]

func _on_resume_button_pressed():
	for node in get_tree().get_nodes_in_group("points_ui"):
		node.visible = false
