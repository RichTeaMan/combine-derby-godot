extends CanvasLayer

func fade() -> void:
	$AnimationPlayer.play("fade")
	await $AnimationPlayer.animation_finished
	
func fade_func(callback_function: Callable, args: Array) -> void:
	$AnimationPlayer.play("fade")
	await $AnimationPlayer.animation_finished
	callback_function.callv(args)

func fade_back() -> void:
	$AnimationPlayer.play_backwards("fade")
