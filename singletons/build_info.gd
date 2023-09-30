extends CanvasLayer

var commit_hash = "dev"
var build_time = "dev"

func _ready():
	$"%build_label".text = "Build hash: %s | Build date: %s" % [ commit_hash, build_time ]

func _process(_delta: float) -> void:
	$"%fps_label".text = "FPS: %s" % Engine.get_frames_per_second()
