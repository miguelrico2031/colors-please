
using System.Collections.Generic;

public interface IPersistenceService : IService
{
    public void Load();
    public void Save();
    public void NewSave();

    public bool TryGetSavedData(out GameData data);
    

}
