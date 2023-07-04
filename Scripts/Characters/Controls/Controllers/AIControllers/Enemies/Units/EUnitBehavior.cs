namespace Characters.Controls.Controllers.AIControllers.Enemies.Units
{
    public enum EUnitBehavior
    {
        Patrol = 1 << 0,
        Investigate = 1 << 1,
        Guard = 1 << 2,
        Engage = 1 << 3,
        Search = 1 << 4,
        ExecuteJob = 1 << 5,
    }
}