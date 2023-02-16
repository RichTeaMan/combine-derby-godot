extends Viewport

export var player_id: int

# Called when the node enters the scene tree for the first time.
func _ready():
	#var root = get_tree().get_current_scene()
	#print("root node: %s" % root.name)
	Global.connect("player_added", self, "_on_player_added")
	print("viewport %d ready" % player_id)

func _on_player_added(added_player_id: int, node: Node):
	if player_id == added_player_id:
		add_child(node)
		print("Player %d successfully added" % player_id)
