using Godot;

[GlobalClass]
[Tool]
public partial class Interstitial : DescriptiveNode
{
    [Export]
    public string ContinueTextOverride;
    [Export]
    public StringName NextNode;
}
