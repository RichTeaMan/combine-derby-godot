extends Spatial

var commit_hash = "dev"
var build_time = "dev"

func _ready():
	$gui/RichTextLabel.text = "Build hash: %s | Build date: %s" % [ commit_hash, build_time ]
