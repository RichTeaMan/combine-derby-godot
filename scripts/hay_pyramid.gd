tool

extends Spatial

export (int, 1, 100) var bales = 1
export (int, 1, 10) var height = 1
export (float, 0, 10, 0.5) var padding = 1.0

var bale_template;

var bale_height = 1.20
var bale_radius = 0.75
var bale_rotation = PI * 0.5


func _enter_tree():
	bale_template = preload("res://hay_bale.tscn")
	create_bales()

# Called when the node enters the scene tree for the first time.
func _process(_delta):
	if Engine.editor_hint:
		create_bales()

func create_bales():
	for node in get_children():
		node.queue_free()
	var y = bale_height / 2.0
	for _height in height:
		# multiply by -0.5 to center bales around node origin
		var x = -0.5 * padding * bales
		for _n in bales:
			var instance = bale_template.instance()
			instance.translation.x = x
			instance.translation.y = y
			instance.rotate_z(bale_rotation)
			add_child(instance)
			x += padding
		y += bale_height
