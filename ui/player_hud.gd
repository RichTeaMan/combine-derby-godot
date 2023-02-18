extends Node

var current_points = 0

var countdown_length_seconds = 60
var game_active = true;

export var player_id = 1;

func _ready():
	var _a = Global.connect("points", self, "_on_points")
	var _b = Global.connect("speed", self, "_on_speed")
	var _c = Global.connect("player_ui", self, "_on_player_ui")
	_on_points(player_id, 0)
	_on_speed(player_id, 0)

func _on_player_ui(player_id: int, ui: Node) -> void:
	print("something rec")
	print("%s -> %s" % [self.player_id, player_id])
	if self.player_id != player_id:
		return
	print("Received ui for player %s" % player_id)
	add_child(ui)

func _on_points(signal_player_id, score):
	if signal_player_id != player_id || !game_active:
		return
	current_points += score
	$gui/points_label.text = "Points: %s" % current_points

func _on_speed(signal_player_id, speed):
	if signal_player_id != self.player_id:
		return
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed
