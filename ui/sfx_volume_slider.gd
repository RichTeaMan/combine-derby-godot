extends HSlider

func _ready():
	self.value = Global.get_sfx_volume()
	print("Set sfx value to %s" % value)

func _on_sfx_volume_slider_value_changed(value):
	Global.set_sfx_volume(value)
