using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [IJumpable]
/// 점프하는 대상이 정해져 있으니
/// </summary>

public interface IJumpable
{
    public float JumpPower { get; } 
    public void Jumping();
}
