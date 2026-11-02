using Godot;

[GlobalClass]
[Tool]
public partial class StoryNode : Resource
{
    [Export]
    public Vector2 GraphPosition;
    [Export]
    public StringName GraphName;
}

