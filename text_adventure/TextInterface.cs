using Godot;
using System;

public partial class TextInterface : Control
{
	Viewport gameViewport;
	[Export] LineEdit input;
	[Export] RichTextLabel output;
	[ExportGroup("Sound Effects")]
	[Export] AudioStream jumpscare;
	[ExportGroup("Jumpscare")]
	[Export] float scaryFaceRevealDuration = 10f;
	[Export] float jumpscarePitch = 0.2f;
	[Export] float foundYouInterval = 2f;
	[Export] float scaryFaceInterval = 5f;
	public const int ScreenWidth = 56;
	public const int ScreenHeight = 15;
	public override void _Ready()
	{
		input.GrabFocus();
		input.Edit();
		gameViewport = GetViewport();
		input.TextSubmitted += OnSubmit;
	}
	void OnSubmit(string text)
	{
		input.Clear();
		//placeholder
		FailMessage();
	}
	public void PrintText(string text)
	{
		output.Text += "\n " + text;
	}
	public void FailMessage()
	{
		output.Text += "\n[color=red]FAIL.[/color]";
		Jumpscare();
	}
	public void ClearScreen()
	{
		output.Text = "";
	}
	public void Jumpscare()
	{
		input.Hide();
		input.Editable = false;
		ClearScreen();
		PrintText("[shake]\n\n\n\n\n\n\n[center]FOUND YOU.[/center][/shake]");
		Tween t = GetTree().CreateTween();
		t.TweenInterval(foundYouInterval);
		t.TweenCallback(Callable.From(() =>
		{
			output.VisibleRatio = 0;
			TV.PlaySound(jumpscare,jumpscarePitch);
			ClearScreen();
			PrintText("[color=red]"+AsciiArt.scaryFace+"[/color]");
		}));
		t.TweenProperty(output,"visible_ratio",1f,scaryFaceRevealDuration);
		t.TweenInterval(scaryFaceInterval);
		t.TweenCallback(Callable.From(() =>
		{
			output.VisibleCharacters = -1;
			ClearScreen();
			output.Text = "todo: more scary stuff / switch to static";
			//Handle game over (jumpscare irl?)
		}));
	}
}
