using Godot;
using Godot.Collections;

[Tool]
public partial class Main : GraphEdit
{
    [Export]
    PopupMenu popupMenu;
    [Export]
    Array<PackedScene> nodes = [];

    public readonly static Shortcut AddNodeShortcut = SetUpShortcut();

    public override void _Ready()
    {
        foreach (var node in nodes)
        {
            popupMenu.AddItem(node.ResourcePath.Split('/')[^1].Split('.')[0].ToPascalCase());
        }
    }

    static Shortcut SetUpShortcut()
    {
        InputEventKey keyEvent = new()
        {
            Keycode = Key.A,
            CtrlPressed = true,
            CommandOrControlAutoremap = true, // Swaps Ctrl for Command on Mac.
        };

        InputEventMouseButton right = new()
        {
            ButtonIndex = MouseButton.Right,
        };

        Shortcut shortcut = new()
        {
            Events = [keyEvent, right]
        };

        return shortcut;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (AddNodeShortcut.MatchesEvent(@event))
        {
            popupMenu.Position = DisplayServer.MouseGetPosition();
            popupMenu.Popup();
        }
    }
}
