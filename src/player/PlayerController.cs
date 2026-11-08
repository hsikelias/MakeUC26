/*
Overview:



*/
using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	[Export]
	public float Speed {get; set;} = 5.0f;
	// add mouse sensitivity and camera pos later here

    public override void _Ready()
	{
		// runs when player enters the scene tree, 
		// capture moves (DO NOT DO MOVEMENT HERE)	
		// setting initial state
		 
		 Input.MouseMode = Input.MouseModeEnum.Captured;
		 GD.Print("mouse hidden and position locked to center");

	}

    public override void _UnhandledInput(InputEvent @event)
	{
		// event based input
		// receives mouse motion: rotating player left/right and
		// camera up/down. This belongs here cus godot sends this shit
		// as "InputEventMouseMotion"

		// the player root is controlling the Y rotation and
		// Camera3D is controlling the X rotation. 

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
        var direction = Vector3.Zero;
		

		// checking for input and updating direction.

    }

}
