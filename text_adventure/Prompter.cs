using Godot;
using Godot.Collections;

public abstract partial class Prompter : Node2D
{
    /// <summary>
    /// Prompt the user with some text and options to choose from. Does not return until the user has selected an option.
    /// </summary>
    /// <param name="body">Main text to display to the user.</param>
    /// <param name="options">Array of options the user can choose from. If none are provided, default text is used.</param>
    /// <returns>The index of the selected option or <b><c>0</c></b> if no options are provided.</returns>
    public abstract int Prompt(string body, Array<string> options);
}
