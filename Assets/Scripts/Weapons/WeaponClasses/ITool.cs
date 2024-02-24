using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool
{
    public ToolData data { get; set; }
    public void Use();
}
