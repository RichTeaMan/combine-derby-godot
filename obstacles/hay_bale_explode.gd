extends Node3D

func _ready():
	$CPUParticles3D.one_shot = true

func _physics_process(_d):
	if not $CPUParticles3D.emitting:
		queue_free()
