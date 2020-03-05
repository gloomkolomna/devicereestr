using System;

namespace DeviceReestr.Core.Entities
{
    public class Device
    {
        public Guid Id { get; set; }
        public string SerialNo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public User Owner { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"SerialNo: {SerialNo}, Type: {Type}, Description: {Description}";
        }
    }
}