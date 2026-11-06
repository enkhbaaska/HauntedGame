using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class SimpleChoiceNodeLogic : GraphNode
{
    [Export]
    private LineEdit firstOption;

    [Export]
    private BaseButton addButton;
    [Export]
    private BaseButton removeButton;

    private int _firstOptionIdx;
    private int _optionCount = 1;

    public override void _Ready()
    {
        _firstOptionIdx = firstOption.GetIndex();
        addButton.Pressed += AddOption;
        removeButton.Pressed += RemoveOption;
    }

    private void AddOption()
    {
        var option = new LineEdit() { PlaceholderText = firstOption.PlaceholderText };
        AddChild(option);
        MoveChild(option, _firstOptionIdx + _optionCount);
        SetSlotEnabledRight(_firstOptionIdx + _optionCount, true);
        _optionCount++;
        if (_optionCount > 1)
        {
            removeButton.Disabled = false;
        }
    }

    public void AddOption(string text)
    {
        AddOption();
        GetChild<LineEdit>(_firstOptionIdx + _optionCount - 1).Text = text;
    }

    public void RemoveOption()
    {
        _optionCount--;
        GetChild(_firstOptionIdx + _optionCount).QueueFree();
        SetSlotEnabledRight(_firstOptionIdx + _optionCount, false);
        if (_optionCount <= 1)
        {
            removeButton.Disabled = true;
        }
    }

    public Array<string> GetOptions()
    {
        Array<string> result = [];
        for (int i = _firstOptionIdx; i < _firstOptionIdx + _optionCount; i++)
        {
            result.Add(GetChild<LineEdit>(i).Text);
        }
        return result;
    }
}
