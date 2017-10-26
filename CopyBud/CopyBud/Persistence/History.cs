using System;

namespace CopyBud.Persistence
{
    public class History
    {
        public int Id { get; set; }
        public string ClipString { get; set; }
        public DateTime DateTimeTaken { get; set; }
    }
}
