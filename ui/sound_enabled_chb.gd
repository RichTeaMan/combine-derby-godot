extends CheckBox

func _ready():
	pressed = !Global.get_mute()
