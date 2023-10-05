extends Node

var countdown_length_seconds = 60
var game_active = true;

@export var player_id = 1;

func _ready():
	var _a = Global.game_info_ui.connect(on_set_game_info_ui)
	var _b = Global.connect("speed", Callable(self, "_on_speed"))
	var _c = Global.connect("player_ui", Callable(self, "_on_player_ui"))
	on_set_game_info_ui(player_id, "")
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

func on_set_game_info_ui(signal_player_id, message: String):
	if signal_player_id != player_id || !game_active:
		return
	$gui/game_info_label.text = message

func _on_speed(signal_player_id, speed):
	if signal_player_id != self.player_id:
		return
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed

func _on_Timer_timeout():
	print("control fade")
	$AnimationPlayer.play("control_fade")
