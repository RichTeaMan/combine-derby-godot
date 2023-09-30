extends HSlider

func _ready():
	self.value = Global.get_music_volume()

func _on_music_volume_slider_value_changed(new_value):	
	Global.set_music_volume(new_value)
