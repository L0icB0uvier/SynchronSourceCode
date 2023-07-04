using InControl;

namespace Characters.Controls.Input.Actions
{
	public class SkullfaceActions : PlayerActionSet
	{
		public PlayerAction RSLeft;
		public PlayerAction RSRight;
		public PlayerAction RSUp;
		public PlayerAction RSDown;
		public TwoAxisInputControl RS;
		public PlayerAction Boost;
		public PlayerAction Teleport;
		public PlayerAction Interact;
		public PlayerAction Follow;

		public SkullfaceActions()
		{
			RSLeft = CreatePlayerAction("Left");
			RSRight = CreatePlayerAction("Right");
			RSUp = CreatePlayerAction("Up");
			RSDown = CreatePlayerAction("Down");
			RS = CreateTwoAxisPlayerAction(RSLeft, RSRight, RSDown, RSUp);
			Boost = CreatePlayerAction("Boost");
			Teleport = CreatePlayerAction("CallBack");
			Interact = CreatePlayerAction("Interact");
			Follow = CreatePlayerAction("Follow");
		}
	}
}
