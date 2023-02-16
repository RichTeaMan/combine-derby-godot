extends Node

# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass

func add_player(player_id: int, node: Node):
	emit_signal("player_added", player_id, node)
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
