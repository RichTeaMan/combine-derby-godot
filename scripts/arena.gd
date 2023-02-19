extends Spatial

func _ready():
	print("Setting up arena.")
	Global.set_playlist(["JasonShaw_JennysTheme.ogg",
		"JasonShaw_Snappy.ogg",
		"jazzyfrenchy.ogg"], true)

	var spawns = fetch_player_spawns()
	var players = get_tree().get_nodes_in_group("player")
	print("    Adding %s players." % players.size())
	print("    Found %s arena spawns." % spawns.size())
	for i in players.size():
		players[i].transform = spawns[i]
		print("    Set transform for player %d at %s." % [i, spawns[i]])
	$combine.queue_free()
	$combine2.queue_free()
	print("Arena setup done.")

func fetch_player_spawns():
	return [ $combine.transform, $combine2.transform ]
