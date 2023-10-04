@tool

extends Node3D

const MAX_INSTANCES = 65536

const COLLISION_CELL_SIZE = 0.5

const MAX_TILE_SIZE = 20.0

## The width of the patch in metres.
@export_range(1, 200, COLLISION_CELL_SIZE) var width: float = 1.0:
	set = set_width

## The height of the patch in metres.
@export_range(1, 200, COLLISION_CELL_SIZE) var height: float = 1.0:
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

func on_patch_body_entered(body: Node3D, multi_mesh: MultiMesh, chunk_map: Array, harvest_map: Array, chunk_index: int) -> void:
	if body is StaticBody3D:
		return
	if harvest_map[chunk_index]:
		return

	print("harvested")
	harvest_map[chunk_index] = true
	for i in chunk_map[chunk_index]:
		var transform = multi_mesh.get_instance_transform(i)
		multi_mesh.set_instance_transform(i, transform.scaled(Vector3.ZERO))
	if "player_id" in body:
		Global.add_points(body.player_id, chunk_map[chunk_index].size(), "wheat")

func create_tiles():
	if !is_inside_tree():
		return
	$generated_tiles.position = Vector3.ZERO
	for node in $generated_tiles.get_children():
		node.queue_free()

	# create array of dimensions
	var width_counter = width
	var widths: Array[float] = []
	while width_counter > MAX_TILE_SIZE:
		widths.append(MAX_TILE_SIZE)
		width_counter -= MAX_TILE_SIZE
	if width_counter > 0:
		widths.append(width_counter)
	var height_counter = height
	var heights: Array[float] = []
	while height_counter > MAX_TILE_SIZE:
		heights.append(MAX_TILE_SIZE)
		height_counter -= MAX_TILE_SIZE
	if height_counter > 0:
		heights.append(height_counter)

	var height_offset = 0
	for h in heights:
		var width_offset = 0
		for w in widths:
			var tile = create_tile(w, h)
			tile.position = Vector3(width_offset, 0.0, height_offset)
			$generated_tiles.add_child(tile)
			## should get automagically renamed?
			tile.name = "wheat_tile_1"
			width_offset += w
		height_offset += h

	# centre main tile
	$generated_tiles.position = Vector3(-width / 2.0, 0.0, -height / 2.0)
	print("Setting at %s" % [$generated_tiles.position])

func create_tile(tile_width: float, tile_height: float) -> Node3D:

	print("Rebuilding %s tile size %s,%s" % [name, tile_width, tile_height])
	
	var tile = Node3D.new()

	# setup collision areas
	var box = BoxShape3D.new()
	box.size = Vector3(COLLISION_CELL_SIZE, 2.0, COLLISION_CELL_SIZE)
	# split area into chunks
	var areas: Array[Area3D] = []
	var chunk_h = int(tile_height / COLLISION_CELL_SIZE)
	var chunk_w = int(tile_width / COLLISION_CELL_SIZE)

	var chunk_count = 0
	## aray of array int. gdscript doesn't have the syntax to strongly type n-dimensional arrays
	var chunk_map = []
	var harvest_map = []

	var multi_mesh = MultiMesh.new()
	for h in range(chunk_h):
		for w in range(chunk_w):
			var collision_shape = CollisionShape3D.new()
			collision_shape.shape = box
			var area = Area3D.new()
			# areas surround their position, eg the origin is the center of their area
			area.position = Vector3(w * COLLISION_CELL_SIZE, 0.0, h * COLLISION_CELL_SIZE)
			area.add_child(collision_shape)

			var local_chunk_id = chunk_count
			area.body_entered.connect(func(body): on_patch_body_entered(body, multi_mesh, chunk_map, harvest_map, local_chunk_id))

			areas.append(area)
			chunk_map.append([])
			harvest_map.append(false)
			chunk_count += 1

	multi_mesh.transform_format = MultiMesh.TRANSFORM_3D	
	multi_mesh.mesh = $Wheat.mesh

	var unused_chunks = []

	multi_mesh.instance_count = tile_width * tile_height * density
	if multi_mesh.instance_count > MAX_INSTANCES:
		push_warning("Wheat patch is creating %s instances, more than the allowed %s. Consider decreasing density." % [multi_mesh.instance_count, MAX_INSTANCES])
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
	
	tile.add_child(mmi)
	for area in areas:
		tile.add_child(area)

	return tile
