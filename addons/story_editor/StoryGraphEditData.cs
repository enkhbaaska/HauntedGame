using Godot;
using Godot.Collections;

/// <summary>
/// Data structure for serialisation of story graph.
/// </summary>
[GlobalClass]
[Tool]
public partial class StoryGraphEditData : Resource
{
    /// <summary>
    /// All the data the story needs to function, including graph connections
    /// </summary>
    [Export]
    public Array<StoryNode> Story = [];

    /// <summary>
    /// 'Godot-form' of graph connections to make reconstruction easier
    /// </summary>
    [Export]
    public Array<Dictionary> Connections = [];
}
