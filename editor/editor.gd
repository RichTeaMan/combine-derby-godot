extends Node3D

var rotating = false
var rotation_constant = 0.5
var zoom_constant = 0.2
var prev_mouse_position
var next_mouse_position

var SELECTED_GRP = "selected"

@onready
var highlighted_shader = preload("res://shaders/highlighted.gdshader")

func _unhandled_input(event):
	if (Input.is_action_just_pressed("rotate")):
		rotating = true
		prev_mouse_position = get_viewport().get_mouse_position()
	if (Input.is_action_just_released("rotate")):
		rotating = false

func _process(delta):
	
	if (rotating):
		next_mouse_position = get_viewport().get_mouse_position()
		%gimbal.rotate_y((next_mouse_position.x - prev_mouse_position.x) * rotation_constant * delta)
		%gimbal.rotate_x((next_mouse_position.y - prev_mouse_position.y) * rotation_constant * delta)
		prev_mouse_position = next_mouse_position
	
	if (Input.is_action_just_pressed("zoom_in")):
		%camera.position.z -= zoom_constant
	
	if (Input.is_action_just_pressed("zoom_out")):
		%camera.position.z += zoom_constant

func _on_button_clear_pressed():
	for c in %container.get_children():
		c.queue_free()

func _on_button_combine_pressed():
	var scene = load("res://vehicles/combine.tscn")
	var instance = scene.instantiate()
	%container.add_child(instance)
	add_selection_listener(instance, instance)

func _on_button_cube_pressed():
	var scene = load("res://vehicles/cube.tscn")
	var instance: Node3D = scene.instantiate()
	var x = randi_range(-10, 10)
	var y = randi_range(-10, 10)
	var z = randi_range(-10, 10)
	%container.add_child(instance)
	instance.scale.x = 0.3
	instance.scale.y = 0.3
	instance.scale.z = 0.3
	instance.position.x = x as float
	instance.position.y = y as float
	instance.position.z = z as float
	add_selection_listener(instance, instance)

func override_material(container: Node3D):
	for c in container.get_children(true):
		if c is MeshInstance3D:
			var mesh_instance: MeshInstance3D = c
			if mesh_instance.mesh.get_surface_count() > 0:
				var material = mesh_instance.mesh.surface_get_material(0)
				material.albedo_color = %ColorPicker.color
				var shader = ShaderMaterial.new()
				shader.shader = highlighted_shader
				shader.set_shader_parameter("outline_color", Color.WHITE)
				
				mesh_instance.material_overlay = shader
		override_material(c)

func remove_override_material(container: Node3D):
	for c in container.get_children(true):
		if c is MeshInstance3D:
			var mesh_instance: MeshInstance3D = c
			mesh_instance.material_overlay = null
		remove_override_material(c)

func add_selection_listener(root: Node3D, container: Node3D):
	for c in container.get_children(true):
		if c is CollisionObject3D:
			var collision: CollisionObject3D = c
			var f = func(_camera: Node, event: InputEvent, _position: Vector3, _normal: Vector3, shape_idx: int):
				var n = root
				_model_clicked(n, event)
			collision.input_event.connect(f)
		if c is RigidBody3D:
			var rigid_body: RigidBody3D = c
			rigid_body.freeze = true
		add_selection_listener(root, c)

func _on_color_picker_color_changed(color):
	override_material(%container)

func _model_clicked(source_node: Node3D, event: InputEvent):
	if event is InputEventMouseMotion:
		pass

	if event is InputEventMouseButton:
		var button_event: InputEventMouseButton = event
		if button_event.button_index == MOUSE_BUTTON_LEFT && button_event.is_released():
			clear_selected()
			override_material(source_node)
			source_node.add_to_group(SELECTED_GRP)

func clear_selected():
	for n in %container.get_tree().get_nodes_in_group(SELECTED_GRP):
		n.remove_from_group(SELECTED_GRP)
		remove_override_material(n)
