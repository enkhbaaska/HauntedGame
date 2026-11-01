using Godot;
using System;

public partial class CameraScript : Camera3D
{
	[Export] float mouseSensitivity = 0.01f;
	[Export] bool horizontalOnly = true;
	Camera3D camera;
	Godot.Collections.Array<Node> spotlist;
	int cameraIndex = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
			parent = GetParent<Node3D>();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;
	}
	Node3D parent;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			parent.RotateY(-((InputEventMouseMotion)@event).Relative.X * mouseSensitivity * 0.1f);
			if (!horizontalOnly)
				RotateX(-((InputEventMouseMotion)@event).Relative.Y * mouseSensitivity * 0.1f);
			RotationDegrees = new Vector3(Mathf.Clamp(RotationDegrees.X, -90, 90), RotationDegrees.Y, RotationDegrees.Z);
			
		}
		if (@event is InputEventMouseButton)
		{
			if (((InputEventMouseButton)@event).IsPressed()) Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
}
