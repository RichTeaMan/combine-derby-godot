extends RichTextLabel

func _ready():
	#var root = get_tree().get_current_scene()
	#root.connect("gameover_update", self, "_on_gameover_update")
	pass

func _on_gameover_update(gameover_dict):
	bbcode_text = "[center]Points: %d[/center]" % [ gameover_dict["score"] ]
