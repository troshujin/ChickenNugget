using Godot;
using System;

public partial class MainGame : Node2D
{
	private Node _currentLevel;

	public override void _Ready()
	{
		// Find the starting level (if you instanced one in the editor)
		_currentLevel = GetNode("LevelRoot").GetChild(0);
	}

	public void LoadLevel(string scenePath, string spawnName = "PlayerSpawn")
	{
		// Remove old level
		if (_currentLevel != null)
			_currentLevel.QueueFree();

		// Load new level
		var packed = GD.Load<PackedScene>(scenePath);
		var newLevel = packed.Instantiate();
		GetNode("LevelRoot").AddChild(newLevel);
		_currentLevel = newLevel;

		// Move player to spawn point
		var spawn = newLevel.GetNodeOrNull<Node2D>(spawnName);
		if (spawn != null)
			GetNode<Node2D>("Player").GlobalPosition = spawn.GlobalPosition;
		else
			GD.PushWarning($"Spawn point '{spawnName}' not found in {scenePath}");
	}
}
