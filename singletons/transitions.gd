extends CanvasLayer

func fade() -> void:
	$AnimationPlayer.play("fade")
	yield($AnimationPlayer, "animation_finished")
	
func fade_func(callback_function: FuncRef, args: Array) -> void:
	$AnimationPlayer.play("fade")
	yield($AnimationPlayer, "animation_finished")
	callback_function.call_funcv(args)

func fade_back() -> void:
	$AnimationPlayer.play_backwards("fade")
