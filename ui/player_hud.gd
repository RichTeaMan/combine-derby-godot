extends Node

var current_points = 0

var countdown_length_seconds = 60
var game_active = true;

export var player_id = 1;

func _ready():
	var _a = Global.connect("points", self, "_on_points")
	var _b = Global.connect("speed", self, "_on_speed")
	_on_points(player_id, 0)
	_on_speed(player_id, 0)

func _on_points(signal_player_id, score):
	if signal_player_id != player_id || !game_active:
		return
	current_points += score
	$gui/points_label.text = "Points: %s" % current_points

func _on_speed(signal_player_id, speed):
	if signal_player_id != self.player_id:
		return
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed

