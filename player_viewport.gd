extends SubViewport

@export var player_id: int

var has_player = false

func _ready():
	Global.player_added.connect(_on_player_added)
	Global.gfx_settings_updated.connect(_on_gfx_settings_updated)
	get_tree().get_root().size_changed.connect(_on_resize)
	_on_resize()
	print("viewport %d ready" % player_id)

func _on_resize():
	call_deferred("apply_scale")

func apply_scale():
	scaling_3d_scale = Global.gfx_scaling
	print("Applied scale %s." % Global.gfx_scaling)

func _on_player_added(added_player_id: int, node: Node):
	if !has_player &&  player_id == added_player_id:
		has_player = true
		add_child(node)
		print("Player %d successfully added" % player_id)

func _on_gfx_settings_updated():
	apply_scale()
