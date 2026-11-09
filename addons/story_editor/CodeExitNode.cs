using Godot;

/// <summary>
/// Node that allows an 'exit' from code, i.e. is triggered by code and feeds into the graph.
/// </summary>
[GlobalClass]
[Tool]
public partial class CodeExitNode : ControlNode
{
    [Export]
    public StringName NextNode;
}

