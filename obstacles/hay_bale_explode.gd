extends Spatial

func _ready():
	$CPUParticles.one_shot = true

func _physics_process(_d):
	if not $CPUParticles.emitting:
		queue_free()
