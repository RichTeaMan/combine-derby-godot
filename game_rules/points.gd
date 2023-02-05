extends Spatial

var current_points = 0

func _ready():
	Global.connect("points", self, "_on_points")
	Global.connect("speed", self, "_on_speed")
	_on_points(0)
	_on_speed(0, 0)

func _enter_tree():
	var arena = preload("res://arena.tscn")
	add_child(arena.instance())

func _on_points(score):
	current_points += score
	$gui/points_label.text = "Points: %s" % current_points

func _on_speed(speed, _vehicle_id):
	$gui2/speed_label.text = "Speed: %.1fm/s" % speed
