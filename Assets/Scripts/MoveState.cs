using System;

[Serializable]
public class MoveState
{
    public string playerName { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public int direction { get; set; }
}
