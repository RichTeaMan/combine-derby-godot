extends HSlider

func _ready():
	value = Global.get_master_volume()
	print("Set value to %s" % value)

func _on_master_volume_slider_value_changed(value):
	value = Global.set_master_volume(value)
