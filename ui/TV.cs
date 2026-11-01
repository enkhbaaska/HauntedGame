using Godot;
using System;

public partial class TV : Node3D
{
    public static TV instance;
	[Export] Viewport screenViewport;
    [Export] AudioStreamPlayer3D effectsAudio;
    public override void _Ready()
    {
        instance = this;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        screenViewport.PushInput(@event);
    }
    public static void PlaySound(AudioStream sfx,float pitch = 1f)
    {
        instance.effectsAudio.Stream = sfx;
        instance.effectsAudio.PitchScale = pitch;
        instance.effectsAudio.Play();
    }
}
