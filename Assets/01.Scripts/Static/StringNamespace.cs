using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringTag
{
    public const string Player = "Player";
}

public static class StringScene
{
    public const string MainScene = "MainScene";
    public const string LoadingScene = "LoadingScene";
}

public static class StringAddressable
{
    public const string Environment = "Environment";
    
    // GUI
    public const string MainScene = "MainScene";
    public const string StartScene = "StartScene";
}

// 아이템 이름 싹다 넣어주기

public static class StringItemOftenUse
{
    public const string Rock = "Rock";
    public const string Wood = "Wood";
    //----오브젝트 풀 테스트용 키 밸류
    
    public static readonly string[] Keys = { Rock, Wood };
}