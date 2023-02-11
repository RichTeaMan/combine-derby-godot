extends Spatial

var small_playing = false
var big_playing = false

var small_sounds
var big_sounds

func _ready():
	randomize()
	small_sounds = $small.get_children()
	big_sounds = $big.get_children()
	for sound in small_sounds:
		sound.connect("finished", self, "_on_small_sound_finshed")
	for sound in big_sounds:
		sound.connect("finished", self, "_on_big_sound_finshed")

func play_small_sound():
	if !small_playing:
		play_random_sound(small_sounds)
		small_playing = true

func play_big_sound():
	if !big_playing:
		play_random_sound(big_sounds)
		big_playing = true

func play_random_sound(sound_array: Array):
	var sound = pick_random_child(sound_array)
	print("playing crash sound: %s" % sound.name)
	sound.play()

func pick_random_child(node_array: Array):
	$small/s3.name
	var random_id =  randi() % node_array.size()
	return node_array[random_id]

func _on_small_sound_finshed():
	small_playing = false
	print("small crash sound stopped playing")

func _on_big_sound_finshed():
	big_playing = false
	print("big crash sound stopped playing")
