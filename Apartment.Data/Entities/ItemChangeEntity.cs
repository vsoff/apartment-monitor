namespace Apartment.Data.Entities
{
    public class ItemChangeEntity : Entity
    {
        public int ObjectId { get; set; }
        public string Table { get; set; }
        public string PropertyTypeFullName { get; set; }
        public string PropertyName { get; set; }
        public string OldValueJson { get; set; }
        public string NewValueJson { get; set; }
    }
}