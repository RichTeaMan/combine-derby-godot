tool

extends Spatial

export (int, 1, 1000) var width = 1

func _enter_tree():
	$MeshInstance.scale.z = width
	pass

func _process(delta):
	if Engine.editor_hint:
		$MeshInstance.scale.z = width
