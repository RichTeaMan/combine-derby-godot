extends CheckBox

func _ready():
	self.button_pressed = !Global.camera_reverses


func _on_checkbox_reverse_camera_gui_input(_event):
	Global.camera_reverses = !self.button_pressed
