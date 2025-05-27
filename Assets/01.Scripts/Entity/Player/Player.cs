using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity
{
    public PlayerController playerController { get; private set; }
    
    void Awake()
    {
        CharacterManager.Register(this);
    }
    
}
