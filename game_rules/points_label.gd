extends RichTextLabel

func _on_gameover_update(gameover_dict):
	bbcode_text = "[center]Points: %d[/center]" % [ gameover_dict["score"] ]
