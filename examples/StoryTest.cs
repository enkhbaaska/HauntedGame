using Godot;
using System;

public partial class StoryTest : Node
{
    [Export]
    StoryReader StoryReader;

    public override void _Ready()
    {
        GD.Print("Once upon a time...");
        StoryReader.Connect("End", Callable.From(End));
        StoryReader.EmitSignal("Start");
    }

    private void End()
    {
        GD.Print("The end...");
    }
}
