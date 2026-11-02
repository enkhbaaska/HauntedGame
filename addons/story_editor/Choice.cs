using Godot;

[GlobalClass]
[Tool]
public partial class Choice : Resource 
{
    [Export]
    public string OptionText;
    [Export]
    public StringName NextNode;
}
