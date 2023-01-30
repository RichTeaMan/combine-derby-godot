tool

extends Spatial


export (int, 1, 50) var length = 1
export (int, 1, 50) var height = 1
export (int, 1, 50) var width = 1

func _enter_tree():
	$MeshInstance.scale.x = length
	$MeshInstance.scale.y = height
	$MeshInstance.scale.z = width
	pass

func _process(_delta):
	if Engine.editor_hint:
		$MeshInstance.scale.x = length
		$MeshInstance.scale.y = height
		$MeshInstance.scale.z = width
