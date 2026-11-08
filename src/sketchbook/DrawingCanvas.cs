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
	
	// ts the most stroke history programs gonna remember.
	private const int MaxUndoSteps = 5;
	private readonly List<List<Vector2>> _undoHistory = new();

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
		if (!_isDrawing)
		{
			return;
		}

		_isDrawing = false;

		if (_currentStroke.Count < 2)
		{
			_strokes.Remove(_currentStroke);
			return;
		}

		_undoHistory.Add(_currentStroke);

		if (_undoHistory.Count > MaxUndoSteps)
		{
			_undoHistory.RemoveAt(0);
		}

	}

	public void UndoLastStroke()
	{
		if (_isDrawing || _undoHistory.Count == 0)
		{
			return;
		}

		List<Vector2> strokeToRemove =
			_undoHistory[_undoHistory.Count - 1];

		_undoHistory.RemoveAt(_undoHistory.Count - 1);
		_strokes.Remove(strokeToRemove);

		QueueRedraw();
	}

	public void ClearPage()
	{
		_isDrawing = false;
		_currentStroke = new List<Vector2>();

		_strokes.Clear();
		_undoHistory.Clear();

		QueueRedraw();
	}
}
