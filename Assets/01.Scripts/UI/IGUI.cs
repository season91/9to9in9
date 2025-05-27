using UnityEngine;

public interface IGUI
{
    GameObject UIObject { get; }
    
    public void Initialization();
    
    public void Open();
    
    public void Close();
}
