using Godot;

/// <summary>
/// User choice that is associated with a resultant node.
/// </summary>
[GlobalClass]
[Tool]
public partial class Choice : Resource 
{
    [Export]
    public string OptionText;
    [Export]
    public StringName NextNode;
}
