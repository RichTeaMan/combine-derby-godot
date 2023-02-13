extends Spatial

var current_points = 0

signal timer_text_update(minutes_remaining, seconds_remaining)
signal gameover_update(gameover_dict)

var countdown_length_seconds = 60
var game_active = true;

func _ready():
	var _a = Global.connect("points", self, "_on_points")
	var _b = Global.connect("speed", self, "_on_speed")
	_on_points(0)
	_on_speed(0, 0)
	refresh_timer()
	$game_over_container.visible = false

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instance())

func _on_points(score):
	if !game_active:
		return
	current_points += score
	$gui/points_label.text = "Points: %s" % current_points

func _on_speed(speed, _vehicle_id):
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed

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
