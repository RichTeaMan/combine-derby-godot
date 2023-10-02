extends Node3D

func _enter_tree():
	var arena = preload("res://arenas/crash.tscn")
	add_child(arena.instantiate())
