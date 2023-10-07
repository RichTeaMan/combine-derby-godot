extends StaticBody3D

const BALE_LIMIT = 1000

var stored_wheat = 0

var bale_rotation = PI * 0.5
var bale_height = 1.20
var spawn_height = 16

@onready var bale_template = preload("res://obstacles/hay_bale.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func deposit_wheat(wheat: int) -> void:
	stored_wheat += wheat
	var bales_to_spawn = 0
	while stored_wheat >= BALE_LIMIT:
		bales_to_spawn += 1
		stored_wheat -= BALE_LIMIT
	print("Created %s bales" % bales_to_spawn)
	var spawn_point = position + Vector3(0, spawn_height + bale_height, 0)
	for i in bales_to_spawn:
		var bale_instance = bale_template.instantiate()
		#bale_instance.rotate_z(bale_rotation)
		bale_instance.position = spawn_point
		self.get_parent_node_3d().add_child(bale_instance)
		print(bale_instance.position)
		spawn_point += Vector3(0, bale_height * 1.25, 0)
