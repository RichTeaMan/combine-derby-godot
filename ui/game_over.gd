extends CenterContainer

export var player_id: int
export var points: int

func _ready():
	$"%points_label".bbcode_text = "Points %s" % points

func _on_resume_button_pressed():
	queue_free()
