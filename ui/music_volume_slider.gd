extends HSlider

func _ready():
	value = Global.get_music_volume()

func _on_music_volume_slider_value_changed(value):
	value = Global.set_music_volume(value)
