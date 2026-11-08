using Godot;
using System;
using System.Runtime.CompilerServices;



public enum SketchbookState
{
    Stowed,
    Held,
    Drawing
}

public partial class SketchbookController : Node3D
{
    private PlayerController _player;
    public override void _Ready()
    {
        // this path is going to sketchbookAnchor/Camera3D/Player
        _player = GetNode<PlayerController>("../../..");
        Visible = false;
        
    }
    private SketchbookState _currentState = SketchbookState.Stowed;
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_sketchbook")
        && _currentState != SketchbookState.Drawing)
            {
                _currentState = _currentState == SketchbookState.Stowed
                    ? SketchbookState.Held
                    : SketchbookState.Stowed;

                Visible = _currentState != SketchbookState.Stowed;
                GD.Print(_currentState);
            }

        if (@event.IsActionPressed("toggle_drawing"))
            {


                if (_currentState == SketchbookState.Held)
                {
                    _currentState = SketchbookState.Drawing;
                    Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
                    
                    _player.SetLookEnabled(false);
                }
                else if (_currentState == SketchbookState.Drawing)
                {
                    _player.SetLookEnabled(true);
                    _currentState = SketchbookState.Held;
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                }

                GD.Print(_currentState);
            }
    }
}