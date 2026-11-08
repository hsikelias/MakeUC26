using Godot;
using System;
using System.Collections.Generic;
public partial class DrawingCanvas : Control
{
	// Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// }

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

    public override void _Draw()
    {
		foreach (List<Vector2> stroke in _strokes)
		{
    		if (stroke.Count >= 2)
    		{
        	DrawPolyline(stroke.ToArray(), Colors.Black, 4.0f, true);
			//drawing pen stroke width.
    		}
		}
    }

	private readonly List<List<Vector2>> _strokes = new();
	private List<Vector2> _currentStroke = new();
	private bool _isDrawing;

	public void BeginStroke(Vector2 point)
	{
		//creates and stores new/independent strokes
    	_currentStroke = new List<Vector2> { point };
    	_strokes.Add(_currentStroke);
    	_isDrawing = true;
	}

	public void AddPoint(Vector2 point)
	{
		// adds movement to a stroke and redrawing them
    	if (!_isDrawing)
    	{
        	return;
    	}
    	_currentStroke.Add(point);
    	QueueRedraw();
	}

	public void EndStroke()
	{
		// stops accepting points to that specific strokee
    	_isDrawing = false;
	}

}
