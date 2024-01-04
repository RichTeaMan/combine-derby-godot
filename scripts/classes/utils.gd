class_name Utils

static func children_recursive(node: Node3D) -> Array[Node3D]:
	var children: Array[Node3D] = []
	for c in node.get_children():
		children.append(c)
		children.append_array(children_recursive(c))
	return children
