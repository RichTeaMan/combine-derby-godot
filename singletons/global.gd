extends Node3D


signal vehicle_pickup(player_id: int, category, quantity: int)

signal vehicle_body_shape_entered(player_id: int, body: Node3D)


signal speed(player_id: int, speed_ms: float)




var current_game_scene: Node
var current_player_count = 1

var current_playlist: Array
var current_playlist_index: int


func do_vehicle_body_shape_entered(player_id: int, body: Node3D) -> void:
    emit_signal("vehicle_body_shape_entered", player_id, body)


func update_speed(player_id: int, speed_ms: float) -> void:
    emit_signal("speed", player_id, speed_ms)
