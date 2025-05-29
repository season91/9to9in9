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
}

public static class StringAdrCraftRecipe
{
    public const string Recipes = "Recipes";
}

#endregion

#endregion

public static class StringItemOftenUse
{
    public const string Rock = "Rock";
    public const string Wood = "Wood";
    //----오브젝트 풀 테스트용 키 밸류
    
    public static readonly string[] Keys = { Rock, Wood };
}

public static class StringItemName
{
    public const string Meat = "meat";
    public const string Apple = "apple";
    public const string MeatCooked = "meat-cooked";
    public const string Orange = "orange";
    public const string Pear = "pear";
    public const string Mushroom = "mushroom";
    public const string ToolAxe = "tool-axe";
    public const string ToolPickaxe = "tool-pickaxe";
    public const string Helmet = "helmet";
    public const string ArmorChest = "armor-chest";
    public const string ArmorLeg = "armor-leg";
    public const string Boots = "boots";
    public const string Shield = "shield";
    public const string Sword = "sword";
    public const string Hammer = "hammer";
    public const string Stone = "stone";
    public const string Wood = "wood";
    public const string Iron = "iron";
    public const string IronBar = "iron-bar";
    public const string WoodenWall = "wooden-wall";
    public const string WoodenFloor = "wooden-floor";
    public const string MetalWall = "metal-wall";
    public const string Workbench = "workbench";
    public const string Campfire = "campfire";
    public const string Smelter = "smelter";
    public const string Anvil = "anvil";
}