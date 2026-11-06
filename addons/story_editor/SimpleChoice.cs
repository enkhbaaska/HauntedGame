using Godot;
using Godot.Collections;

[GlobalClass]
[Tool]
public partial class SimpleChoice : DescriptiveNode
{
    [Export]
    public Array<Choice> Choices;
}

