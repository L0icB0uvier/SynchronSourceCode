namespace Characters.CharacterAbilities.AbilitiesPower
{
    public interface IAbilitiesPower
    {
        float MaxPower { get; }
        
        bool IsRecharging { get; set; }
        
        bool IsEmpty { get; }
        void ConsumePower(float powerConsumed);
    }
}