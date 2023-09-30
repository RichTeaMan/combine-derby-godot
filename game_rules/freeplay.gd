extends Node3D

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instantiate())
