using Godot;

/// <summary>
/// Node that serves to break linear sequences up with fixed user input. Similar to a choice node but with only one option.
/// </summary>
[GlobalClass]
[Tool]
public partial class Interstitial : DescriptiveNode
{
    /// <summary>
    /// Optional override for the default "continue" text
    /// </summary>
    [Export]
    public string ContinueTextOverride;
    [Export]
    public StringName NextNode;
}
