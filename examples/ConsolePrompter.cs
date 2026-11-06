using Godot;
using Godot.Collections;
using System;

/// <summary>
/// An example of a <c>Prompter</c> that logs to the console and randomly selects player choices
/// </summary>
public partial class ConsolePrompter : Prompter
{
    [Export]
    public string DefaultContinueText = "Continue.";

    RandomNumberGenerator rng = new();

    public override int Prompt(string body, Array<string> options)
    {
        Array<string> interpolatedOptions = options.Count == 0 ? [DefaultContinueText] : options;

        GD.Print(body);
        for (int i = 0; i < interpolatedOptions.Count; i++)
        {
            GD.Print($"{i + 1}. {interpolatedOptions[i]}");
        }
        System.Threading.Thread.Sleep(500);
        int selected = rng.RandiRange(0, interpolatedOptions.Count - 1);
        GD.Print($"Rng chose '{interpolatedOptions[selected]}'");
        return selected;
    }
}
