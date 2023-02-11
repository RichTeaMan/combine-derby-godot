extends Spatial

var current_points = 0

signal timer_text_update(minutes_remaining, seconds_remaining)

var countdown_length_seconds = 300

func _ready():
	Global.connect("points", self, "_on_points")
	Global.connect("speed", self, "_on_speed")
	_on_points(0)
	_on_speed(0, 0)
	refresh_timer()

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instance())

func _on_points(score):
	current_points += score
	$gui/points_label.text = "Points: %s" % current_points

func _on_speed(speed, _vehicle_id):
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed

func _on_Timer_timeout():
	if countdown_length_seconds <= 0:
		return
	countdown_length_seconds += -1
	refresh_timer()

func refresh_timer():
	emit_signal("timer_text_update", countdown_length_seconds / 60 , countdown_length_seconds % 60)
