using Godot;

/// <summary>
/// Node that emits or recieves a signal.
/// </summary>
[GlobalClass]
[Tool]
public abstract partial class ControlNode : StoryNode
{
    [Export]
    public string StorySignalName;
}