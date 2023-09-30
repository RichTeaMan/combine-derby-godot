extends CenterContainer

@export var player_id: int
@export var points: Dictionary

func _ready():
	$"%points_label".text = "Points %s" % points["points"]
	var row_height = 50
	var key_cell_width = 180
	var value_cell_width = 40
	for p in points:
		if p == "points":
			continue
		var key_label = RichTextLabel.new();
		key_label.text = p
		key_label.custom_minimum_size.x = key_cell_width
		key_label.custom_minimum_size.y = row_height
		key_label.theme = $"%points_label".theme
		var value_label = RichTextLabel.new();
		value_label.text = "%s" % points[p]
		value_label.custom_minimum_size.x = value_cell_width
		value_label.custom_minimum_size.y = row_height
		value_label.theme = $"%points_label".theme
		key_label.add_theme_font_size_override("normal_font_size", 24)
		value_label.add_theme_font_size_override("normal_font_size", 24)
		$"%points_breakdown_grid".add_child(key_label)
		$"%points_breakdown_grid".add_child(value_label)

func _on_resume_button_pressed():
	for node in get_tree().get_nodes_in_group("points_ui"):
		node.visible = false
	queue_free()

func _on_restart_button_pressed():
	var callback = Callable(self, "restart_game")
	Transitions.fade_func(callback, [])

func restart_game():
	Global.do_restart_game()
	Transitions.fade_back()
