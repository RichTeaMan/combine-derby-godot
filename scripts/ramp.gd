@tool

extends Node3D


@export_range(1, 50) var length: int = 1
@export_range(1, 50) var height: int = 1
@export_range(1, 50) var width: int = 1

func _enter_tree():
	$MeshInstance3D.scale.x = length
	$MeshInstance3D.scale.y = height
	$MeshInstance3D.scale.z = width
	pass

func _process(_delta):
	if Engine.is_editor_hint():
		$MeshInstance3D.scale.x = length
		$MeshInstance3D.scale.y = height
		$MeshInstance3D.scale.z = width
