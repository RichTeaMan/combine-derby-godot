extends Spatial

var current_points = 0

signal timer_text_update(minutes_remaining, seconds_remaining)
signal gameover_update(gameover_dict)

var countdown_length_seconds = 60
var game_active = true;

func _ready():
	refresh_timer()
	$game_over_container.visible = false

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instance())

func _on_Timer_timeout():
	if countdown_length_seconds <= 0:
		game_active = false
		$game_over_container.visible = true
		$Timer.stop()
		emit_signal("gameover_update", {"score": current_points})
		return
	countdown_length_seconds += -1
	refresh_timer()

func refresh_timer():
	emit_signal("timer_text_update", countdown_length_seconds / 60 , countdown_length_seconds % 60)


func _on_resume_button_pressed():
	$game_over_container.visible = false
	for node in get_tree().get_nodes_in_group("points_ui"):
		node.visible = false
