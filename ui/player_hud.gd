extends Node

var current_points = 0

var countdown_length_seconds = 60
var game_active = true;

@export var player_id = 1;

func _ready():
	var _a = Global.connect("points", Callable(self, "_on_points"))
	var _b = Global.connect("speed", Callable(self, "_on_speed"))
	var _c = Global.connect("player_ui", Callable(self, "_on_player_ui"))
	_on_points(player_id, 0, "")
	_on_speed(player_id, 0)

	var template = "[center]Control with %s keys.[/center]"
	var control_text = ""
	if player_id == 1:
		control_text = template % "WASD"
	elif player_id == 2:
		control_text = template % "IJKL"
	$"%control_label".text = control_text

func _on_player_ui(singal_player_id: int, ui: Node) -> void:
	if self.player_id != singal_player_id:
		return
	print("Received ui for player %s" % self.player_id)
	add_child(ui)

func _on_points(signal_player_id, score, _category):
	if signal_player_id != player_id || !game_active:
		return
	current_points += score
	$gui/points_label.text = "    Points: %s" % current_points

func _on_speed(signal_player_id, speed):
	if signal_player_id != self.player_id:
		return
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed

func _on_Timer_timeout():
	print("control fade")
	$AnimationPlayer.play("control_fade")
