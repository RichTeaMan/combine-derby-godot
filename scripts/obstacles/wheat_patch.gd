@tool

extends Node3D

const MAX_INSTANCES = 65536

const COLLISION_CELL_SIZE = 0.5

## The width of the patch in metres.
@export_range(1, 20, COLLISION_CELL_SIZE) var width: float = 1.0:
	set = set_width

## The height of the patch in metres.
@export_range(1, 20, COLLISION_CELL_SIZE) var height: float = 1.0:
	set = set_height

## The density of wheat per square metre.
@export_range(1, 20, 1) var density: float = 6.0:
	set = set_density

func set_width(new_width):
	if width != new_width:
		width = new_width
		create_tiles()

func set_height(new_height):
	if height != new_height:
		height = new_height
		create_tiles()

func set_density(new_density):
	if density != new_density:
		density = new_density
		create_tiles()

func _enter_tree():
	create_tiles()

func on_patch_body_entered(body: Node3D, multi_mesh: MultiMesh, chunk_map: Array, chunk_index: int) -> void:
	if body is StaticBody3D:
		return
	print("body entered DA WHEAT #%s" % chunk_index)
	print(body.name)
	for i in chunk_map[chunk_index]:
		var transform = multi_mesh.get_instance_transform(i)
		multi_mesh.set_instance_transform(i, transform.scaled(Vector3.ZERO))
	pass

func create_tiles():
	if !is_inside_tree():
		return

	print("Rebuilding %s" % name)
	
	for node in $generated_tiles.get_children():
		node.queue_free()
		
	# setup collision areas
	var box = BoxShape3D.new()
	box.size = Vector3(COLLISION_CELL_SIZE, 2.0, COLLISION_CELL_SIZE)
	# split area into chunks
	var areas: Array[Area3D] = []
	var chunk_h = int(height / COLLISION_CELL_SIZE)
	var chunk_w = int(width / COLLISION_CELL_SIZE)
	
	var chunk_count = 0
	## aray of array int. gdscript doesn't have the syntax to strongly type n-dimensional arrays
	var chunk_map = []
	var multi_mesh = MultiMesh.new()
	for h in range(chunk_h):
		for w in range(chunk_w):
			var collision_shape = CollisionShape3D.new()
			collision_shape.shape = box
			var area = Area3D.new()
			# areas surround their position, eg the origin is the center of their area
			area.position = Vector3((w * COLLISION_CELL_SIZE) - (width / 2.0), 0.0, (h * COLLISION_CELL_SIZE) - (height / 2.0))
			print(area.position)
			area.add_child(collision_shape)
			
			area.body_entered.connect(on_patch_body_entered)
			var local_chunk_id = chunk_count
			area.body_entered.connect(func(body): on_patch_body_entered(body, multi_mesh, chunk_map, local_chunk_id))
			areas.append(area)
			chunk_map.append([])
			chunk_count += 1
	
	multi_mesh.transform_format = MultiMesh.TRANSFORM_3D	
	multi_mesh.mesh = $Wheat.mesh
	
	var unused_chunks = []
	
	multi_mesh.instance_count = width * height * density
	print("Creating %s instances" % multi_mesh.instance_count)
	for i in multi_mesh.instance_count:
		if unused_chunks.size() == 0:
			unused_chunks = range(chunk_h * chunk_w)
		
		var chunk_index = randi() % unused_chunks.size()
		var chunk_id = unused_chunks[chunk_index]
		unused_chunks.remove_at(chunk_index)
		
		var chunk_x = areas[chunk_id].position.x
		var chunk_y = areas[chunk_id].position.z
		
		var h = (chunk_x + randf_range(-COLLISION_CELL_SIZE / 2, COLLISION_CELL_SIZE / 2))
		var w = (chunk_y + randf_range(-COLLISION_CELL_SIZE / 2, COLLISION_CELL_SIZE / 2))
		var vec = Vector3(h, 0.0, w)
		var instance_transform = Transform3D()\
			.scaled(Vector3(randf_range(0.9, 1.1), randf_range(0.9, 1.1), randf_range(0.9, 1.1)))\
			.rotated(Vector3.UP, randf_range(-PI/2, PI/2))
		instance_transform.origin = vec
		chunk_map[chunk_id].append(i)
		multi_mesh.set_instance_transform(i, instance_transform)
	
	var mmi = MultiMeshInstance3D.new()
	mmi.multimesh = multi_mesh
	
	$generated_tiles.add_child(mmi)
	for area in areas:
		$generated_tiles.add_child(area)
		pass
	
	print("Rebuilding %s complete" % name)
	
	#var mmi = MultiMeshInstance3D.new()
	#mmi.
	#multi_mesh.inst
	#multi_mesh.get_instance_transform(0)
	
	#var padding = 0.5
	#var start_height = -height / 2
	
	#var source_tiles = $source_tiles.get_children()

	#for h in range(start_height, start_height + height):
	#	var start_width = -width / 2
	#	for w in range(start_width, start_width + width):
	#		var instance = source_tiles[randi() % source_tiles.size()].duplicate()
	#		instance.position.x = h * padding
	#		instance.position.z = w * padding
	#		$generated_tiles.add_child(instance)
