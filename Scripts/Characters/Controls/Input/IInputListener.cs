namespace Characters.Controls.Input
{
    public interface IInputListener
    {
        bool ListenInputs { get; }
        
        void AssignActionsBinding();

        void ReadInputs();
        
        void EnableInputs();
        void DisableInputs();
    }
}