extends Spatial

func _ready():
	Global.set_playlist(["JasonShaw_JennysTheme.ogg",
		"JasonShaw_Snappy.ogg",
		"jazzyfrenchy.ogg"], true)

	print("READY")
	var spawns = fetch_player_spawns()
	var players = get_tree().get_nodes_in_group("player")
	print("looping %s times" % players.size())
	print("spawns %s" % spawns.size())
	for i in players.size():
		print(i)
		var trans = spawns[i]
		players[i].transform = trans
		print("Set transform for player %d at %s." % [i, spawns[i]])
	print("LOOP DONE")
	$combine.queue_free()
	$combine2.queue_free()
	#get_tree().quit()

func fetch_player_spawns():
	return [ $combine.transform, $combine2.transform ]
