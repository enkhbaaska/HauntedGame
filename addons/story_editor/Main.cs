using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class Main : GraphEdit
{
    [Export]
    PopupMenu popupMenu;
    [Export]
    Array<PackedScene> nodes = [];
    [Export]
    FileDialog fileDialogue;

    string loadedFilePath;

    public readonly static Shortcut AddNodeShortcut = SetUpShortcut();

    public override void _Ready()
    {
        foreach (var node in nodes)
        {
            popupMenu.AddItem(node.ResourcePath.Split('/')[^1].Split('.')[0].ToPascalCase());
        }

        popupMenu.IndexPressed += PopupPressed;
        fileDialogue.FileSelected += OnFileSelected;

        SetUpFileButtons();
    }

    private void SetUpFileButtons()
    {
        Button save = new()
        {
            Text = "Save"
        };
        Button open = new()
        {
            Text = "Open..."
        };

        save.Pressed += OnSavePressed;
        open.Pressed += OpenFile;

        HBoxContainer toolbar = GetMenuHBox();
        toolbar.AddChild(save);
        toolbar.AddChild(open);
    }

    private void OnSavePressed()
    {
        if (string.IsNullOrEmpty(loadedFilePath))
        {
            SaveAs();
        }
        else
        {
            Save(loadedFilePath);
        }

    }

    private void Save(string path)
    {
        StoryGraphEditData graph = new()
        {
            Story = ConvertToStoryNodes(),
            Connections = GetConnectionList()
        };
        GD.Print($"Saving {graph.Story.Count} nodes...");

        var saveErr = ResourceSaver.Save(graph, path);
        if (saveErr != Error.Ok)
        {
            GD.PrintErr($"Error saving: {saveErr}");
        }
    }

    private Array<StoryNode> ConvertToStoryNodes()
    {
        Array<StoryNode> nodes = [];

        foreach (var node in GetChildren())
        {
            // The following is a really ugly & fragile method of collecting all the nodes.
            // In future each node should have a script that implements some kind of ToStoryNode() function.
            if (node is GraphNode g)
            {
                var connections = GetConnectionListFromNode(g.Name);
                switch (g.Title)
                {
                    case "CodeEnter":
                        nodes.Add(new CodeEnterNode()
                        {
                            GraphName = g.Name,
                            GraphPosition = g.PositionOffset,
                            StorySignalName = g.GetNode<LineEdit>("LineEdit").Text,
                        });
                        break;
                    case "CodeExit":
                        nodes.Add(new CodeExitNode()
                        {
                            GraphName = g.Name,
                            GraphPosition = g.PositionOffset,
                            StorySignalName = g.GetNode<LineEdit>("LineEdit").Text,
                        });
                        break;
                    case "Interstitial":
                        nodes.Add(new Interstitial()
                        {
                            GraphName = g.Name,
                            GraphPosition = g.PositionOffset,
                            BodyText = g.GetNode<TextEdit>("Body").Text,
                            ContinueTextOverride = g.GetNode<LineEdit>("LineEdit").Text
                        });
                        break;
                    case "SimpleChoice":
                        var logic = g as SimpleChoiceNodeLogic;
                        Array<Choice> choices = [];
                        var options = logic.GetOptions();
                        List<(int index, StringName name)> optionConnections = [];
                        foreach (var connection in connections)
                        {
                            if (connection["from_node"].AsStringName() == g.Name)
                            {
                                optionConnections.Add(new() { 
                                    index = connection["from_port"].AsInt32(), 
                                    name = connection["to_node"].AsStringName() 
                                });
                            }
                        }
                        optionConnections.Sort((a, b) => a.index.CompareTo(b));
                        for (int i = 0; i < options.Count; i++)
                        {
                            Choice choice = new()
                            {
                                OptionText = options[i],
                            };
                            if (i < optionConnections.Count)
                            {
                                choice.NextNode  = optionConnections[i].name;
                            }
                        }
                        nodes.Add(new SimpleChoice()
                        {
                            GraphName = g.Name,
                            GraphPosition = g.PositionOffset,
                            BodyText = g.GetNode<TextEdit>("Body").Text,
                            Choices = choices
                        });
                        break;
                    default:
                        GD.PrintErr("Graph node type not recognised");
                        break;
                }
            }
        }

        return nodes;
    }

    private void LoadNodesFromStoryNodes(Array<StoryNode> storyNodes)
    {
        // Don't need to set up connections in here, done by caller.
        foreach (var node in storyNodes)
        {
            GraphNode graphNode = null;
            if (node is CodeExitNode exit)
            {
                graphNode = AddNode(0);
                graphNode.GetNode<LineEdit>("LineEdit").Text = exit.StorySignalName;
            }
            else if (node is CodeEnterNode enter)
            {
                graphNode = AddNode(1);
                graphNode.GetNode<LineEdit>("LineEdit").Text = enter.StorySignalName;
            }
            else if (node is SimpleChoice choices)
            {
                graphNode = AddNode(2);
                foreach (Choice choice in choices.Choices) {
                    (graphNode as SimpleChoiceNodeLogic).AddOption(choice.OptionText);
                }
            }
            else if (node is Interstitial inter)
            {
                graphNode = AddNode(3);
                graphNode.GetNode<TextEdit>("Body").Text = inter.BodyText;
                graphNode.GetNode<LineEdit>("LineEdit").Text = inter.ContinueTextOverride;
            }
            else
            {
                GD.PrintErr("Story node type not recognised");
                continue;
            }
            graphNode.PositionOffset = node.GraphPosition;
            graphNode.Name = node.GraphName;
        }
    }

    private void SaveAs()
    {
        fileDialogue.FileMode = FileDialog.FileModeEnum.SaveFile;
        fileDialogue.CurrentFile = "story.tres";
        fileDialogue.PopupCentered();
    }

    private void OpenFile()
    {
        fileDialogue.FileMode = FileDialog.FileModeEnum.OpenFile;
        fileDialogue.PopupCentered();
    }

    private void OnFileSelected(string path)
    {
        if (fileDialogue.FileMode == FileDialog.FileModeEnum.SaveFile)
        {
            loadedFilePath = path;
            Save(path);
        }
        else if (fileDialogue.FileMode == FileDialog.FileModeEnum.OpenFile)
        {
            loadedFilePath = path;
            StoryGraphEditData graph = ResourceLoader.Load<StoryGraphEditData>(path, "StoryGraphEditData");

            // Remove existing graph nodes.
            foreach (Node child in GetChildren())
            {
                if (child is GraphNode)
                    child.Free();
            }

            ClearConnections();
            LoadNodesFromStoryNodes(graph.Story);

            foreach (var connection in graph.Connections)
            {
                ConnectNode(
                    connection["from_node"].AsStringName(),
                    connection["from_port"].AsInt32(),
                    connection["to_node"].AsStringName(),
                    connection["to_port"].AsInt32()
                );
            }
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

    private void PopupPressed(long index)
    {
        AddNode((int)index);
    }

    private GraphNode AddNode(int index)
    {
        GraphNode newNode = nodes[index].Instantiate<GraphNode>();
        AddChild(newNode);
        newNode.PositionOffset = (GetLocalMousePosition() + ScrollOffset) / Zoom;
        return newNode;
    }

    private void OnDeleteRequest(Array<StringName> nodes)
    {
        foreach (var node in nodes)
        {
            GetNode(node.ToString()).QueueFree();
        }
    }

    private void OnConnectionRequest(StringName fromNode, int fromPort, StringName toNode, int toPort)
    {
        ConnectNode(fromNode, fromPort, toNode, toPort);
    }

    private void OnDisconnectRequest(StringName fromNode, int fromPort, StringName toNode, int toPort)
    {
        DisconnectNode(fromNode, fromPort, toNode, toPort);
    }
}
