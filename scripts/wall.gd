@tool

extends Node3D

@export_range(1, 1000, 1) var length:int = 1
var current_length = 0

var wall_chunk_template;

var padding = 1.0


func _enter_tree():
	wall_chunk_template = preload("res://wall_chunk.tscn")
	create_wall_chunks()

func _process(_delta):
	if Engine.is_editor_hint() && length != current_length:
		create_wall_chunks()
		current_length = length

func create_wall_chunks():
	for node in get_children():
		node.queue_free()
	# multiply by -0.5 to center wall chunks around node origin
	var z = -0.5 * padding * length
	for _n in length:
		var instance = wall_chunk_template.instantiate()
		instance.position.z = z
		add_child(instance)
		z += padding
