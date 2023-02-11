extends Spatial

var playing = false;

var small_sounds
var big_sounds

func _ready():
	randomize()
	small_sounds = $small.get_children()
	big_sounds = $big.get_children()
	for sound in small_sounds:
		sound.connect("finished", self, "_on_sound_finshed")
	for sound in big_sounds:
		sound.connect("finished", self, "_on_sound_finshed")

func play_small_sound():
	play_random_sound(small_sounds)

func play_big_sound():
	play_random_sound(big_sounds)

func play_random_sound(sound_array: Array):
	if !playing:
		var sound = pick_random_child(sound_array)
		print("playing %s" % sound.name)
		sound.play()
		playing = true

func pick_random_child(node_array: Array):
	$small/s3.name
	var random_id =  randi() % node_array.size()
	return node_array[random_id]

func _on_sound_finshed():
	playing = false
	print("stopped playing")
