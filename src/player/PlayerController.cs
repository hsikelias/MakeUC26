/*
Overview:



*/
using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	[Export]
	public float Speed {get; set;} = 12.0f;
	private Camera3D _camera;

	[Export]
	public float MouseSensitivity {get; set;} = 0.002f;
	private float _cameraPitch = 0.0f; // remembers current vertical angle b/w events
	
    public override void _Ready()
	{
		// runs when player enters the scene tree, 
		// capture moves (DO NOT DO MOVEMENT HERE)	
		// setting initial state
		 
		 Input.MouseMode = Input.MouseModeEnum.Captured;
		 GD.Print("mouse hidden and position locked to center");

		 _camera = GetNode<Camera3D>("Camera3D");
		 _cameraPitch = _camera.Rotation.X;


	}

    public override void _UnhandledInput(InputEvent @event)
	{
		// event based input
		// receives mouse motion: rotating player left/right and
		// camera up/down. This belongs here cus godot sends this shit
		// as "InputEventMouseMotion"

		// the player root is controlling the Y rotation and
		// Camera3D is controlling the X rotation. 



		// logic purely for mouse mode. 

		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GD.Print("pressed esc");
		}

		if (@event is InputEventMouseButton mouseButton
			&& mouseButton.ButtonIndex == MouseButton.Left
			&& mouseButton.Pressed
			&& Input.MouseMode != Input.MouseModeEnum.Captured
		)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			GD.Print("back");
		}

		// vertical mouse motion and horizontal movemnt
		if (Input.MouseMode == Input.MouseModeEnum.Captured
			&& @event is InputEventMouseMotion mouseMotion)
		{
			RotateY(-mouseMotion.Relative.X * MouseSensitivity);
			_cameraPitch -= mouseMotion.Relative.Y * MouseSensitivity;
			
			_cameraPitch = Mathf.Clamp(
				_cameraPitch,
				Mathf.DegToRad(-60.0f),
				Mathf.DegToRad(60.0f)
			);
			_camera.Rotation = new Vector3(_cameraPitch, 0, 0);
		}	






		// if (@event is InputEventMouseMotion mouseMotion)
		// {
		// 	GD.Print(mouseMotion.Relative);
		// }
	}


    public override void _PhysicsProcess(double delta)
    {
		// runs at fixed physics rate and handles the physical movement
		// movement like WASD


		// local variable to store input direction

		Vector2 input = Input.GetVector(
			"move_left",
			"move_right",
			"move_forward",
			"move_backward"
		);
		
		Vector3 localDirection = new Vector3(input.X, 0, input.Y);
		Vector3 direction = (Transform.Basis * localDirection).Normalized();

		Vector3 velocity = Velocity;
		velocity.X = direction.X * Speed;
		velocity.Z = direction.Z * Speed;

		if (!IsOnFloor())
		{
    		velocity += GetGravity() * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
		// checking for input and updating direction.

    }

}
