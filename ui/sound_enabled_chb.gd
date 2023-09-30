extends CheckBox

func _ready():
	button_pressed = !Global.get_mute()
