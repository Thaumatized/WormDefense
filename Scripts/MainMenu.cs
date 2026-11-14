using Godot;

public partial class MainMenu : Control
{
    [Export]
	PackedScene gameplayScene;

	public void startGame()
	{
        GetTree().Root.AddChild(gameplayScene.Instantiate());
		this.QueueFree();
	}

	public void exitGame()
	{
		GetTree().Quit();
	}
}
