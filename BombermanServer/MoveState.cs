namespace BombermanServer
{
    // Bu sınıf, ağ üzerinden gidecek hareket verisidir.
    public class MoveState
    {
        public string PlayerName { get; set; } = ""; // Hareketi kim yaptı?
        public float X { get; set; }           // Nerede (X)?
        public float Y { get; set; }           // Nerede (Y)?
        public int Direction { get; set; }     // Hangi yöne bakıyor? (Animasyon için)
    }
}