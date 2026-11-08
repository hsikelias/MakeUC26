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
    private DrawingCanvas _drawingCanvas;
    private static readonly Vector2 CanvasSize = new Vector2(2048, 1352);
    public override void _Ready()
    {
        // pathjs following SketchBook/DrawingViewport/DrawingCanvas
        _drawingCanvas = GetNode<DrawingCanvas>(
        "DrawingViewport/DrawingCanvas"
        );
        // this path is going to sketchbookAnchor/Camera3D/Player
        _player = GetNode<PlayerController>("../../..");
        Visible = false;
        
    }


    //converts main window pointer position
    private Vector2 MapScreenToCanvas(Vector2 screenPosition)
    {
        Vector2 windowSize = GetViewport().GetVisibleRect().Size;

        return new Vector2(
            screenPosition.X / windowSize.X * CanvasSize.X,
            screenPosition.Y / windowSize.Y * CanvasSize.Y
        );
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
        if (_currentState == SketchbookState.Drawing
            && @event is InputEventMouseButton mouseButton
            && mouseButton.ButtonIndex == MouseButton.Left)
        {
            Vector2 canvasPoint = MapScreenToCanvas(mouseButton.Position);
            if (mouseButton.Pressed)
            {
                _drawingCanvas.BeginStroke(canvasPoint);
            }
            else
            {
                _drawingCanvas.EndStroke();
            }
        }

        if (_currentState == SketchbookState.Drawing
            && @event.IsActionPressed("undo_stroke"))
        {
            _drawingCanvas.UndoLastStroke();
        }

        if (_currentState == SketchbookState.Drawing
            && @event.IsActionPressed("new_page"))
        {
            _drawingCanvas.ClearPage();
        }

        if (_currentState == SketchbookState.Drawing
            && @event is InputEventMouseMotion mouseMotion
            && (mouseMotion.ButtonMask & MouseButtonMask.Left) != 0)
        {
            Vector2 canvasPoint = MapScreenToCanvas(mouseMotion.Position);
            _drawingCanvas.AddPoint(canvasPoint);
        }
    }
}