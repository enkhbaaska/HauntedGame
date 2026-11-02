using Godot;

/// <summary>
/// Base class for nodes in the narrative story. Contains information to allow loading into the graph editor.
/// </summary>
[GlobalClass]
[Tool]
public abstract partial class StoryNode : Resource
{
    [Export]
    public Vector2 GraphPosition;
    [Export]
    public StringName GraphName;
}

