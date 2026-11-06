using Godot;

/// <summary>
/// Node with some text to display to the player
/// </summary>
[GlobalClass]
[Tool]
public abstract partial class DescriptiveNode : StoryNode
{
    [Export]
    public string BodyText;
}