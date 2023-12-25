extends HSlider

func _ready():
	self.value = Global.get_master_volume()
	print("Set value to %s" % value)

func _on_master_volume_slider_value_changed(value):
	Global.set_master_volume(value)
