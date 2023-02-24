extends Viewport

export var player_id: int

var has_player = false

func _ready():
	Global.connect("player_added", self, "_on_player_added")
	Global.connect("gfx_settings_updated", self, "_on_gfx_settings_updated")
	get_tree().get_root().connect("size_changed", self, "_on_resize")
	apply_scale()
	print("viewport %d ready" % player_id)

func _on_resize():
	call_deferred("apply_scale")

func apply_scale():
	size = get_parent().get_rect().size * Global.gfx_scaling
	print("Applied scale.")

func _on_player_added(added_player_id: int, node: Node):
	if !has_player &&  player_id == added_player_id:
		has_player = true
		add_child(node)
		print("Player %d successfully added" % player_id)

func _on_gfx_settings_updated():
	apply_scale()
