using InControl;

namespace Characters.Controls.Input.Actions
{
	public class HicksActions : PlayerActionSet
	{
		public PlayerAction LSLeft;
		public PlayerAction LSRight;
		public PlayerAction LSDown;
		public PlayerAction LSUp;
		public PlayerTwoAxisAction LS;
		public PlayerAction LSButton;

		public PlayerAction Pause;
		
		public PlayerAction StealthMode;
		public PlayerAction Teleport;

		public PlayerAction Interact;

		public HicksActions()
		{
			LSLeft = CreatePlayerAction("Move Left");
			LSRight = CreatePlayerAction("Move Right");
			LSDown = CreatePlayerAction("Move Down");
			LSUp = CreatePlayerAction("Move Up");
			LS = CreateTwoAxisPlayerAction(LSLeft, LSRight, LSDown, LSUp);
			LSButton = CreatePlayerAction("LSButton");

			Pause = CreatePlayerAction("Pause");
			StealthMode = CreatePlayerAction("Stealth Mode");
			Teleport = CreatePlayerAction("Teleport");
			Interact = CreatePlayerAction("Interact");
		}
	}
}
