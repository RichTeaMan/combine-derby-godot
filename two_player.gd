extends Node

func _ready():
	pass

func add_player(player_id: int, node: Node):
	emit_signal("player_added", player_id, node)
