
using Godot;

public partial class CameraMovement : CharacterBody3D
{
	[Export]
	public float MouseSensitivity = 0.002f;

	private Camera3D camera;
	private float verticalRotation = 0.0f;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		camera.Current = true;

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion &&
				Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(-motion.Relative.X * MouseSensitivity);

			verticalRotation -= motion.Relative.Y * MouseSensitivity;

			verticalRotation = Mathf.Clamp(
					verticalRotation,
					Mathf.DegToRad(-89),
					Mathf.DegToRad(89)
			);

			camera.Rotation = new Vector3(verticalRotation, 0, 0);
		}


		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}


		if (@event is InputEventMouseButton button &&
				button.Pressed &&
				button.ButtonIndex == MouseButton.Left)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
}
