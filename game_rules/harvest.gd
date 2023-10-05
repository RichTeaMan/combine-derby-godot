extends Node3D

@export var player_count: int

const WHEAT_LIMIT = 5000.0

const PROP_WHEAT = "wheat"
const PROP_BALES = "bales"

var current_points = 0
var countdown_length_seconds = 60
var game_active = true
var player_points: Array

func set_game_info(player_id: int):
	var wheat: int = player_points[player_id][PROP_WHEAT]
	var filled = (wheat / WHEAT_LIMIT) * 100.0
	Global.set_game_info_ui(player_id, "    Wheat storage: %1.0f%%" % filled)

func _ready():
	refresh_timer()
	Global.vehicle_pickup.connect(on_vehicle_pickup)
	Global.vehicle_body_shape_entered.connect(on_vehicle_body_shape_entered)
	for player_id in player_count:
		set_game_info(player_id + 1)

func _enter_tree():
	player_points = []
	for i in player_count + 1:
		player_points.append({
			PROP_WHEAT: 0,
			PROP_BALES: 0,
		})

func on_vehicle_pickup(player_id: int, category: String, quantity: int) -> void:
	if category == "wheat" && player_id <= player_count:
		player_points[player_id][PROP_WHEAT] += quantity
		if player_points[player_id][PROP_WHEAT] > WHEAT_LIMIT:
			player_points[player_id][PROP_WHEAT] = WHEAT_LIMIT
		set_game_info(player_id)

func on_vehicle_body_shape_entered(player_id: int, body: Node3D) -> void:
	print("harvest: on vehicle body shape entered")

	if body.has_method("deposit_wheat"):
		print("deposit wheat")
		body.deposit_wheat(player_points[player_id][PROP_WHEAT])
		player_points[player_id][PROP_WHEAT] = 0
		set_game_info(player_id)

func _on_Timer_timeout():
	return
	#if countdown_length_seconds <= 0:
	#	game_active = false
	#	$Timer.stop()
	#	var game_over_scene = preload("res://ui/game_over.tscn")

	#	for player_index in player_count:
	#		var player_id = player_index + 1
	#		var game_over_instance = game_over_scene.instantiate()
	#		game_over_instance.player_id = player_id
	#		game_over_instance.points = player_points[player_id]
	#		Global.add_player_ui(player_id, game_over_instance)
	#	return
	#countdown_length_seconds += -1
	#refresh_timer()

func refresh_timer():
	var minutes_remaining = countdown_length_seconds / 60
	var seconds_remaining = countdown_length_seconds % 60
	$"%time_label".text = "[center]%s:%02d[/center]" % [ minutes_remaining, seconds_remaining ]
