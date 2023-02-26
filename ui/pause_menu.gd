extends MarginContainer

var preloaded = false

func _ready():
	print("ready, %s " % Global.gfx_scaling)
	$"%gfx_scaling_slider".set_value(Global.gfx_scaling)
	preloaded = true

	for n in get_tree().get_nodes_in_group("html5_disable"):
		n.visible = not Global.is_web()

func _input(_event):
	if not Global.is_web() && Input.is_action_just_pressed("menu_quit"):
		print("Quit key pressed")
		get_tree().quit()

func _on_gfx_scaling_slider_value_changed(value):
	if not preloaded:
		return
	print("gfx range %s" % $"%gfx_scaling_slider".value)
	Global.gfx_scaling = $"%gfx_scaling_slider".value
	Global.do_gfx_settings_updated()

func _on_button_restart_pressed():
	Global.close_pause()
	Global.do_restart_game()
	Transitions.fade_back()

func _on_button_quit_pressed():
	print("Quit button pressed")
	get_tree().quit()
