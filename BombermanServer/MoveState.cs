using System;

[Serializable]
public class MoveState
{
    // DİKKAT: Sunucudan gelen JSON verisi "küçük harf" olduğu için
    // burayı da küçük harf yapıyoruz. Yoksa veri 0 gelir!
    
    public string playerName { get; set; } // P -> p
    public float x { get; set; }           // X -> x
    public float y { get; set; }           // Y -> y
    public int direction { get; set; }     // D -> d
}
