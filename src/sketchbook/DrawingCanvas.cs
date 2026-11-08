using Godot;
using System;
using System.Collections.Generic;

public enum DrawingTool
{
	Pen,
	Eraser
}

// class stores a strokes points and the apeparence
public sealed class DrawingStroke
{
    public List<Vector2> Points { get; } = new();
    public Color Color { get; }
    public float Width { get; }

    public DrawingStroke(Color color, float width)
    {
        Color = color;
        Width = width;
    }
}

public partial class DrawingCanvas : Control
{

	private static readonly Color PaperColor =
    new Color(0.86390287f, 0.86782753f, 0.8776147f, 1.0f);

	private const float PenWidth = 2.5f;
	private const float EraserWidth = 40.0f;
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
		foreach (DrawingStroke stroke in _strokes)
		{
    		if (stroke.Points.Count >= 2)
			{
				DrawPolyline(
					stroke.Points.ToArray(),
					stroke.Color,
					stroke.Width,
					true
				);
			}
		}
    }

	private readonly List<DrawingStroke> _strokes = new();

	public DrawingTool CurrentTool { get; private set; } =
    DrawingTool.Pen;

	private DrawingStroke _currentStroke =
    new DrawingStroke(Colors.Black, PenWidth);

	private bool _isDrawing;

	private const int MaxUndoSteps = 5;
	private readonly List<DrawingStroke> _undoHistory = new();

	public void BeginStroke(Vector2 point)
	{
    	Color color = CurrentTool == DrawingTool.Pen
        	? Colors.Black
        	: PaperColor;

    	float width = CurrentTool == DrawingTool.Pen
        	? PenWidth
        	: EraserWidth;

    	_currentStroke = new DrawingStroke(color, width);
    	_currentStroke.Points.Add(point);

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
    	_currentStroke.Points.Add(point);
    	QueueRedraw();
	}

	public void EndStroke()
	{
		if (!_isDrawing)
		{
			return;
		}

		_isDrawing = false;

		if (_currentStroke.Points.Count < 2)
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

		DrawingStroke strokeToRemove =
			_undoHistory[_undoHistory.Count - 1];

		_undoHistory.RemoveAt(_undoHistory.Count - 1);
		_strokes.Remove(strokeToRemove);

		QueueRedraw();
	}

	public void ClearPage()
	{
		_isDrawing = false;
		_currentStroke = new DrawingStroke(
			Colors.Black,
			PenWidth
		);

		_strokes.Clear();
		_undoHistory.Clear();

		QueueRedraw();
	}
	public void ToggleTool()
	{
    	CurrentTool = CurrentTool == DrawingTool.Pen
        	? DrawingTool.Eraser
        	: DrawingTool.Pen;

    	GD.Print($"Drawing tool: {CurrentTool}");
	}

	public void SavePage()
{
    const string saveDirectory = "user://sketches";
    string absoluteDirectory =
        ProjectSettings.GlobalizePath(saveDirectory);

    if (!DirAccess.DirExistsAbsolute(absoluteDirectory))
    {
        Error directoryError =
            DirAccess.MakeDirRecursiveAbsolute(absoluteDirectory);

        if (directoryError != Error.Ok)
        {
            GD.PrintErr($"Could not create sketch directory: {directoryError}");
            return;
        }
    }

    string timestamp = DateTime.Now.ToString(
        "yyyy-MM-dd_HH-mm-ss-fff"
    );

    string savePath =
        $"{saveDirectory}/sketch_{timestamp}.png";

    Image image = GetViewport().GetTexture().GetImage();
    Error saveError = image.SavePng(savePath);

    if (saveError == Error.Ok)
    {
        GD.Print(
            $"Sketch saved to: {ProjectSettings.GlobalizePath(savePath)}"
        );
    }
    else
    {
        GD.PrintErr($"Could not save sketch: {saveError}");
    }
}
}
