using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItemData  : ItemData
{
    // 여러 개 가지는 것에 대한 표시
    public bool isStackable; // 여러개 가질 수 있는 아이템인지 구분
    public int maxStack; // 얼마나
}
