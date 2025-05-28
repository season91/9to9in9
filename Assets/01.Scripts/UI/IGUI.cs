using UnityEngine;

public interface IGUI
{
    GameObject GUIObject { get; }
    
    public void Initialization();
    
    public void Open();
    
    public void Close();
}
