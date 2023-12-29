extends Node3D

var rotating = false
var rotation_constant = 0.5
var zoom_constant = 0.2
var prev_mouse_position
var next_mouse_position

@onready
var highlighted_shader = preload("res://shaders/highlighted.gdshader")

func _unhandled_input(event):
	if (Input.is_action_just_pressed("rotate")):
		rotating = true
		prev_mouse_position = get_viewport().get_mouse_position()
	if (Input.is_action_just_released("rotate")):
		rotating = false

func _process(delta):
	# cheap hack to stop gravity being a thing
	for c: Node3D in %container.get_children():
		c.transform = Transform3D.IDENTITY
	
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
	instance.set_process(false)
	instance.set_physics_process(false)

func _on_button_cube_pressed():
	var scene = load("res://vehicles/cube.tscn")
	var instance = scene.instantiate()
	%container.add_child(instance)
	instance.set_process(false)
	instance.set_physics_process(false)

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

func _on_color_picker_color_changed(color):
	override_material(%container)

func dir(class_instance):
	var output = {}
	var methods = []
	for method in class_instance.get_method_list():
		methods.append(method.name)

	output["METHODS"] = methods

	var properties = []
	for prop in class_instance.get_property_list():
		if prop.type == 3:
			properties.append(prop.name)
	output["PROPERTIES"] = properties

	return output
