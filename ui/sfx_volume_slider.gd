extends HSlider

func _ready():
	value = Global.get_sfx_volume()
	print("Set sfx value to %s" % value)

func _on_master_volume_slider_value_changed(value):
	value = Global.set_sfx_volume(value)
