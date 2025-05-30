using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringTag
{
    public const string Player = "Player";
}

public static class StringScene
{
    public const string StartScene = "StartScene";
    public const string MainScene = "MainScene";
    public const string LoadingScene = "LoadingScene";
}

#region  Addressable

public static class StringAdr
{
    public const string Environment = "Environment";
    
    // GUI
    public const string MainScene = "CanvasMainScene";
    public const string StartScene = "CanvasStartScene";
}

#region  ItemData
public static class StringAdrItemDataBuild
{
    // Build
    public const string Anvil = "AnvilData";
    public const string Campfire = "CampfireData";
    public const string MetalWall = "MetalWallData";
    public const string Smelter = "SmelterData";
    public const string WoodenFloor = "WoodenFloorData";
    public const string WoodenWall = "WoodenWallData";
    public const string Workbench = "WorkbenchData";
}

public static class StringAdrItemDataConsumable
{
    // Consumable
    public const string Apple =  "AppleData";
    public const string MeatCooked =  "MeatCookedData";
    public const string Meat =  "MeatData";
    public const string Mushroom =  "MushroomData";
    public const string Orange =  "OrangeData";
    public const string Pear =  "PearData";
}

public static class StringAdrItemDataEquipable
{
    // Equipable
    public const string ArmorChest =  "ArmorChestData";
    public const string ArmorLeg =  "ArmorLegData";
    public const string Boots =  "BootsData";
    public const string Hammer =  "HammerData";
    public const string Helmet =  "HelmetData";
    public const string Shield =  "ShieldData";
    public const string Sword =  "SwordData";
    public const string ToolAxe =  "ToolAxeData";
    public const string ToolPickaxe =  "ToolPickaxeData";
}

public static class StringAdrItemDataResource
{
    // Resource
    public const string Stone =  "StoneData";
    public const string Wood =  "WoodData";
    public const string Iron =  "IronData";
    public const string IronBar =  "IronBarData";
    public const string Tree = "TreeData";
    public const string IronOre = "IronOreData";
    public const string Rock = "RockData";
}

public static class StringAdrCraftRecipe
{
    public const string Recipes = "Recipes";
}

public static class StringEnemyPrefab
{
    public const string Zombie1 = "Zombie1";
    public const string Zombie2 = "Zombie2";
}

public static class StringAdrSpawnManagerKey
{
    // 1차 자원
    public static readonly string[] PoolItem =
    {
        StringAdrItemDataResource.Stone, StringAdrItemDataResource.Wood, StringAdrItemDataResource.Tree, StringAdrItemDataResource.Iron, 
        StringAdrItemDataResource.Rock
    };

    public static readonly string[] PrefabItem =
    {
        //Consumable
        StringAdrItemDataConsumable.Apple, StringAdrItemDataConsumable.MeatCooked, StringAdrItemDataConsumable.Meat,
        StringAdrItemDataConsumable.Mushroom, StringAdrItemDataConsumable.Orange, StringAdrItemDataConsumable.Pear,
        //Equipable
        StringAdrItemDataEquipable.ArmorChest, StringAdrItemDataEquipable.ArmorLeg, StringAdrItemDataEquipable.Boots,
        StringAdrItemDataEquipable.Hammer, StringAdrItemDataEquipable.Helmet, StringAdrItemDataEquipable.Shield,
        StringAdrItemDataEquipable.Sword, StringAdrItemDataEquipable.ToolAxe, StringAdrItemDataEquipable.ToolPickaxe,
        //Resource
        StringAdrItemDataResource.IronBar, StringAdrItemDataResource.IronOre,
        //Build
        StringAdrItemDataBuild.Anvil, StringAdrItemDataBuild.Campfire, StringAdrItemDataBuild.MetalWall,
        StringAdrItemDataBuild.Smelter, StringAdrItemDataBuild.WoodenFloor, StringAdrItemDataBuild.WoodenWall,
        StringAdrItemDataBuild.Workbench,
    };

    public static readonly string[] PrefabEnemy =
    {
        //Enemy
        StringEnemyPrefab.Zombie1, StringEnemyPrefab.Zombie2
    };
}
        

#endregion

#endregion


public static class StringSpawnDic
{
    
    // 자원 종류와 개수 매핑
    public static readonly Dictionary<string, int> resourceSpawnTable = new Dictionary<string, int>
    {
        { StringAdrItemDataResource.Tree, 5 },
        { StringAdrItemDataResource.IronOre, 5 },
        { StringAdrItemDataResource.Rock, 5 },
    };

    public static readonly Dictionary<string, int> enemyResourceSpawnTable = new Dictionary<string, int>
    {
        { StringEnemyPrefab.Zombie1, 1 },
        { StringEnemyPrefab.Zombie2, 2 },
    };

}
