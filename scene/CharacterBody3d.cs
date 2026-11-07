
using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	private Camera3D camera;

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float MouseSensitivity = 0.002f;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		camera.MakeCurrent();

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Mouse movement
		if (@event is InputEventMouseMotion motion &&
				Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Look left and right
			RotateY(-motion.Relative.X * MouseSensitivity);

			// Look up and down
			float verticalRotation =
					camera.Rotation.X - motion.Relative.Y * MouseSensitivity;

			verticalRotation = Mathf.Clamp(
					verticalRotation,
					Mathf.DegToRad(-89),
					Mathf.DegToRad(89)
			);

			camera.Rotation = new Vector3(verticalRotation, 0, 0);
		}

		// Release mouse with ESC
		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		// Capture mouse again with left click
		if (@event is InputEventMouseButton button &&
				button.ButtonIndex == MouseButton.Left &&
				button.Pressed)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// WASD movement
		Vector2 inputDir = Input.GetVector(
				"ui_left",
				"ui_right",
				"ui_up",
				"ui_down"
		);

		Vector3 direction = (
				Transform.Basis *
				new Vector3(inputDir.X, 0, inputDir.Y)
		).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(
					velocity.X, 0, Speed * (float)delta * 10
			);

			velocity.Z = Mathf.MoveToward(
					velocity.Z, 0, Speed * (float)delta * 10
			);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
