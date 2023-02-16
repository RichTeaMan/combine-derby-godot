extends RichTextLabel

func _ready():
	var root = $"../../.."
	root.connect("timer_text_update", self, "_on_timer_refresh")

func _on_timer_refresh(minutes_remaining, seconds_remaining):
	bbcode_text = "[center]%s:%02d[/center]" % [ minutes_remaining, seconds_remaining ]
