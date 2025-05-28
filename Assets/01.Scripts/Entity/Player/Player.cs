using System;
using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity
{
    public PlayerController playerController { get; private set; }
    public PlayerInventoryController inventoryController { get; private set; }
    
    void Awake()
    {
        CharacterManager.Register(this);
        playerController = GetComponent<PlayerController>();
        inventoryController = GetComponent<PlayerInventoryController>();
    }

    private void Start()
    {
        SpawnManager temp = SpawnManager.Instance;
    }
}
