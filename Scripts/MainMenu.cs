using Godot;

public partial class MainMenu : Control
{
	[Export]
	PackedScene gameplayScene;
	[Export]
	PackedScene testScene;

	public void startGame()
	{
		GetTree().Root.AddChild(gameplayScene.Instantiate());
		this.QueueFree();
	}
	public void testGame()
	{
		GetTree().Root.AddChild(testScene.Instantiate());
		this.QueueFree();
	}

	public void exitGame()
	{
		GetTree().Quit();
	}
}
