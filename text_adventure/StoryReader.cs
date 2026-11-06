using Godot;
using Godot.Collections;
using System.Linq;

/// <summary>
/// Recieves story signals and provides the <c>Prompter</c> with text to show the user. When a story sequence is complete, the signal specified in that branch's <c>CodeEnter</c> is emitted.
/// </summary>
public partial class StoryReader : Node
{
    [Export]
    StoryGraphEditData Story;
    [Export]
    Prompter Prompter;

    private Dictionary<string, CodeExitNode> _signalMap = [];
    private Dictionary<string, StoryNode> _nameMap = [];

    public override void _Ready()
    {
        foreach (StoryNode node in Story.Story)
        {
            _nameMap.Add(node.GraphName, node);
            if (node is ControlNode control)
            {
                AddUserSignal(control.StorySignalName);
                Connect(control.StorySignalName, Callable.From(() => ReceiveSignal(control.StorySignalName)));

                if (control is CodeExitNode exit)
                {
                    _signalMap.Add(exit.StorySignalName, exit);
                }
            }
        }
    }

    /// <summary>
    /// Process a story signal emitted from code. <br/>
    /// Must not be called in parallel. This could occur if a <c>CodeExit</c> and <c>CodeEnter</c> node shared the same signal name.
    /// </summary>
    /// <param name="name">Name of the signal, specified by the <c>CodeExit</c> node.</param>
    private void ReceiveSignal(string name)
    {
        if (!_signalMap.TryGetValue(name, out CodeExitNode start))
        {
            GD.PrintErr($"StoryReader did not recognise signal '{name}'");
            return;
        }

        ExecuteSequence(start);
    }

    /// <summary>
    /// Execute a branch of the story starting from a <c>CodeExitNode</c> and continuing until a <c>CodeEnterNode</c> is reached.
    /// </summary>
    /// <param name="node">Exit from code to start from.</param>
    private void ExecuteSequence(CodeExitNode node)
    {
        StringName nextName = node.GraphName;
        while (_nameMap.TryGetValue(nextName, out StoryNode next))
        {
            switch (next)
            {
                case CodeExitNode exit:
                    nextName = exit.NextNode;
                    break;
                case CodeEnterNode enter:
                    ExecuteCodeEnter(enter);
                    nextName = "";
                    break;
                case SimpleChoice choice:
                    nextName = ExecuteChoice(choice);
                    break;
                case Interstitial inter:
                    nextName = ExecuteInterstitial(inter);
                    break;
                default:
                    GD.PrintErr($"Type of node '{next.GraphName}' ({next.GetType().Name}) was not recognised!");
                    nextName = "";
                    break;
            }
        }
        if (!string.IsNullOrEmpty(nextName))
        {
            GD.PrintErr($"Failed to find node '{nextName}' in map!");
        }
    }

    private StringName ExecuteChoice(SimpleChoice choice)
    {
        Array<string> options = [..choice.Choices.Select(c => c.OptionText)];
        int selection = Prompter.Prompt(choice.BodyText, options);
        return choice.Choices[selection].NextNode;
    }

    private StringName ExecuteInterstitial(Interstitial interstitial)
    {
        Array<string> options= string.IsNullOrEmpty(interstitial.ContinueTextOverride) ? [] : [interstitial.ContinueTextOverride];
        Prompter.Prompt(interstitial.BodyText, options);
        return interstitial.NextNode;
    }

    private void ExecuteCodeEnter(CodeEnterNode codeEnter)
    {
        EmitSignal(codeEnter.StorySignalName);
    }
}
