using GeneralScriptableObjects.Events;

namespace SavingSystem
{
    public interface ISavable
    {
        public string SaveKey { get; }
        public string Filepath { get; }
        public void GetSaveInfo();
        public void Save();
        public void Load();

        public void Initialize();
    }

    public interface IGuidSavable : ISavable
    {
        public ObjectUniqueIdentifier Guid { get; }
    }
}