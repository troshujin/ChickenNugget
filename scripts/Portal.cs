using Godot;
using System;

[Tool]
public partial class Portal : Area2D
{
    [Export(PropertyHint.File, "*.tscn")]
    public string TargetScenePath { get; set; } = "";

    [Export]
    public string TargetSpawnName { get; set; } = "PlayerSpawn";

    [Export]
    public NodePath SpritePath { get; set; } = "Sprite2D";

    [Export]
    public Texture2D PortalTexture
    {
        get => _portalTexture;
        set
        {
            _portalTexture = value;
            if (IsInsideTree() && _sprite != null)
                _sprite.Texture = _portalTexture;
        }
    }
    private Texture2D _portalTexture;
    private Sprite2D _sprite;
    private bool _teleporting;

    public override void _Ready()
    {
        _sprite = GetNodeOrNull<Sprite2D>(SpritePath);
        UpdateSpriteTexture();

        if (!Engine.IsEditorHint()) // only connect in game
            BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        // This ensures updates in editor when properties change
        if (Engine.IsEditorHint())
            UpdateSpriteTexture();
    }

    private void UpdateSpriteTexture()
    {
        // if is null, then equals...
        _sprite ??= GetNodeOrNull<Sprite2D>(SpritePath);

        if (_sprite != null)
            _sprite.Texture = _portalTexture;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_teleporting || !body.IsInGroup("Player"))
            return;

        _teleporting = true;

        // Store spawn info
        GetTree().SetMeta("PortalSpawn", TargetSpawnName);

        // Defer the scene change so it's NOT done during the physics callback
        CallDeferred(nameof(DoChangeScene));
    }

    private void DoChangeScene()
    {
        var mainGame = GetNode<MainGame>("/root/MainGame");
        mainGame.LoadLevel(TargetScenePath, TargetSpawnName);
    }
}
