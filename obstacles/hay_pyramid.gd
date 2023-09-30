@tool

extends Node3D

@export_range(0, 10, 0.1) var padding: float = 1.0

@export_range(1, 100) var width: int = 4:
	set = set_width
@export_range(1, 100) var depth: int = 4:
	set = set_depth
@export_range( 1, 100) var max_height: int = 4:
	set = set_max_height

@export_range(-1.0, 1.0, 0.1) var width_offset: float = -0.5:
	set = set_width_offset
@export_range(-1.0, 1.0, 0.1) var depth_offset: float = -0.5:
	set = set_depth_offset

var bale_height = 1.20
var bale_rotation = PI * 0.5

func set_width(new_width):
	if width != new_width:
		width = new_width
		create_bales()

func set_depth(new_depth):
	if depth != new_depth:
		depth = new_depth
		create_bales()

func set_max_height(new_max_height):
	if max_height != new_max_height:
		max_height = new_max_height
		create_bales()

func set_width_offset(new_width_offset):
	if width_offset != new_width_offset:
		width_offset = new_width_offset
		create_bales()

func set_depth_offset(new_depth_offset):
	if depth_offset != new_depth_offset:
		depth_offset = new_depth_offset
		create_bales()

func _enter_tree():
	create_bales()

func create_bales():
	print("Rebuilding %s" % name)
	for node in get_children():
		node.queue_free()

	var bale_template = preload("res://obstacles/hay_bale.tscn")
	var y = bale_height / 2.0

	for h in max_height:
		var current_depth = depth - h
		var current_width = width - h
		if current_depth <= 0:
			current_depth = 1
		if current_width <= 0:
			current_width = 1
		var z = width_offset * padding * current_depth
		for _d in current_depth:
			var x = depth_offset * padding * current_width
			for _w in current_width:
				var instance = bale_template.instantiate()
				instance.position.x = x
				instance.position.y = y
				instance.position.z = z
				instance.rotate_z(bale_rotation)
				add_child(instance)
				x += padding
			z += padding
		y += bale_height
