using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IDamageble, IMoveable, IJumpable, IAttackable
/// </summary>
public class Player : Entity
{
    public PlayerController playerController { get; private set; }
    public PlayerInventoryController inventoryController { get; private set; }
    
    void Awake()
    {//getcomponent에서 null 반환 시 addcomponent할 것
        CharacterManager.Register(this);
        playerController = GetComponent<PlayerController>();
        inventoryController = GetComponent<PlayerInventoryController>();
    }

    private void Start()
    {
        SpawnManager temp = SpawnManager.Instance;
    }
}
