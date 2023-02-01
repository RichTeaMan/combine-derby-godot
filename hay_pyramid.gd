tool

extends Spatial

export (int, 1, 100) var bales = 1;

export (float, 0, 10, 0.5) var padding = 1;

var bale_template;

func _enter_tree():
	bale_template = preload("res://hay_bale.tscn")
	create_bales()
	

# Declare member variables here. Examples:
# var a = 2
# var b = "text"



# Called when the node enters the scene tree for the first time.
func _process(delta):
	if Engine.editor_hint:
		create_bales()

func create_bales():
	for node in get_children():
		node.queue_free()
	var x = -0.5 * padding * bales;
	for _n in bales:
		var instance = bale_template.instance()
		instance.translation.x = x
		instance.translation.y = 0.75
		instance.rotate_z(PI * 0.5)
		add_child(instance)
		x += padding;
