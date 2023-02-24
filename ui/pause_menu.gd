extends MarginContainer

var preloaded = false

func _ready():
	print("ready, %s " % Global.gfx_scaling)
	$"%gfx_scaling_slider".set_value(Global.gfx_scaling)
	preloaded = true

func _input(_event):
	if Input.is_action_just_pressed("menu_quit"):
		print("Quit key pressed")
		get_tree().quit()

func _on_gfx_scaling_slider_value_changed(value):
	if not preloaded:
		return
	print("gfx range %s" % $"%gfx_scaling_slider".value)
	Global.gfx_scaling = $"%gfx_scaling_slider".value
	Global.do_gfx_settings_updated()
