namespace UserManagement.Shared.Interface;

public interface ILocalStorageManager
{
    Task<string> GetItem(string key);
    Task SetItem(string key, string value);
    Task RemoveItem(string key);
}
