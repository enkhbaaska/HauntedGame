using Godot;
using System;

public partial class StoryTest : Node
{
    [Export]
    StoryReader StoryReader;

    public override void _Ready()
    {
        GD.Print("Once upon a time...");
        // When the story reader emits 'End', call End().
        StoryReader.Connect("End", Callable.From(End));
        // Tell the story reader to start the story.
        StoryReader.EmitSignal("Start");
    }

    private void End()
    {
        GD.Print("The end...");
    }
}
