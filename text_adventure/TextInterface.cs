using Godot;
using System;

public partial class TextInterface : Control
{
	Viewport gameViewport;
	[Export] LineEdit input;
	[Export] RichTextLabel output;
	[ExportGroup("Sound Effects")]
	[Export] AudioStream jumpscare;
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
		t.TweenInterval(2);
		t.TweenCallback(Callable.From(() =>
		{
			output.VisibleRatio = 0;
			TV.PlaySound(jumpscare,0.1f);
			ClearScreen();
			PrintText("[color=red]"+AsciiArt.scaryFace+"[/color]");
		}));
		t.TweenProperty(output,"visible_ratio",1f,10f);
		t.TweenInterval(5);
		t.TweenCallback(Callable.From(() =>
		{
			output.VisibleCharacters = -1;
			ClearScreen();
			output.Text = "todo: more scary stuff";
			//Handle game over (jumpscare irl?)
		}));
	}
}
