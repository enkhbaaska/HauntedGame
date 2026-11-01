using Godot;
using System;

public partial class CameraScript : Camera3D
{
	[Export] float mouseSensitivity = 0.01f;
	[Export] Node3D cameraSpots;
	[Export] bool playerMode = false;
	Camera3D camera;
	Godot.Collections.Array<Node> spotlist;
	int cameraIndex = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		if (cameraSpots != null) spotlist = cameraSpots.GetChildren();
		if (!playerMode)
		{
			camera = GetChild<Camera3D>(0);
		}
		else
		{
			parent = GetParent<Node3D>();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;
		if (cameraSpots != null)
		{
			if (Input.IsActionJustPressed("camera_back"))
			{
				cameraIndex--;
				if (cameraIndex < 0) cameraIndex = spotlist.Count - 1;
				parent.GlobalPosition = ((Node3D)spotlist[cameraIndex]).GlobalPosition;
			}
			if (Input.IsActionJustPressed("camera_next"))
			{
				cameraIndex++;
				if (cameraIndex >= spotlist.Count) cameraIndex = 0;
				parent.GlobalPosition = ((Node3D)spotlist[cameraIndex]).GlobalPosition;
			}
		}
	}
	Node3D parent;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (!playerMode)
			{
				RotateY(-((InputEventMouseMotion)@event).Relative.X * mouseSensitivity * 0.1f);
				camera.RotateX(-((InputEventMouseMotion)@event).Relative.Y * mouseSensitivity * 0.1f);
				camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -90, 90), camera.RotationDegrees.Y, camera.RotationDegrees.Z);
			}
			else
			{
				parent.RotateY(-((InputEventMouseMotion)@event).Relative.X * mouseSensitivity * 0.1f);
				RotateX(-((InputEventMouseMotion)@event).Relative.Y * mouseSensitivity * 0.1f);
				RotationDegrees = new Vector3(Mathf.Clamp(RotationDegrees.X, -90, 90), RotationDegrees.Y, RotationDegrees.Z);
			}
		}
		if (@event is InputEventMouseButton)
		{
			if (((InputEventMouseButton)@event).IsPressed()) Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
}
