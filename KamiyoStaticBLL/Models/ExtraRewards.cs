using System.Collections.Generic;

namespace KamiyoStaticBLL.Models
{
    public class ExtraRewards
    {
        public string MessageId { get; set; }
        public List<DropIdQuantity> DroppedBooks { get; set; }
        public List<LorId> DroppedKeypages { get; set; }
    }

    public class DropIdQuantity
    {
        public LorId BookId { get; set; }
        public int Quantity { get; set; }
    }
}
