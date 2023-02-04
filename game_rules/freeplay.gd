extends Spatial

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instance())
