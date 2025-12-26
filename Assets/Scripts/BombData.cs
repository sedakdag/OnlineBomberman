using System;

namespace Contracts
{
    [Serializable]
    public class BombData
    {
        public string bombId  { get; set; }
        public string ownerId { get; set; }
        public float x        { get; set; }
        public float y        { get; set; }
        public int power      { get; set; }
        public float fuseTimeMs { get; set; }
    }
}
