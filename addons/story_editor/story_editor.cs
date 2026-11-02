#if TOOLS
using Godot;
using System;
using static Godot.EditorDock;

/// <summary>
/// Plugin for editing the text-based adventure. Used the plugin tutorial here: https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html
/// </summary>
[Tool]
public partial class story_editor : EditorPlugin
{
    GraphEdit editor;
    EditorDock _dock;

    public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		editor = ResourceLoader.Load<PackedScene>("res://addons/story_editor/main.tscn").Instantiate<GraphEdit>();
		editor.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

        // Create the dock and add the loaded scene to it.
        _dock = new EditorDock();
        _dock.AddChild(editor);

        _dock.Title = "Story editor";

        _dock.DefaultSlot = EditorDock.DockSlot.Bottom;

        // Allow the dock to be on the left or right of the editor, and to be made floating.
        _dock.AvailableLayouts = DockLayout.Horizontal | DockLayout.Floating;

        AddDock(_dock);
    }

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
        RemoveDock(_dock);
        _dock?.QueueFree();
		editor?.QueueFree();
	}
}

#endif
