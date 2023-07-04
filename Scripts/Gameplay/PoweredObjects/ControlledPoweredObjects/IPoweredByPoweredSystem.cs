namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    public interface IPoweredByPoweredSystem
    {
        PoweredSystem PoweredSystem { get; }
        void ControlledElementPowered();
        void ControlledElementUnpowered();
    }
}