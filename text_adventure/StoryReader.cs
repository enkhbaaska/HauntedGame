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

    private Dictionary<string, StoryNode> _nameMap = [];

    public override void _Ready()
    {
        foreach (StoryNode node in Story.Story)
        {
            _nameMap.Add(node.GraphName, node);
            if (node is ControlNode control)
            {
                AddUserSignal(control.StorySignalName);

                if (control is CodeExitNode exit)
                {
                    Connect(exit.StorySignalName, Callable.From(() => ExecuteSequence(exit)));
                }
            }
        }
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
