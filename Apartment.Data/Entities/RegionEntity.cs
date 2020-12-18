namespace Apartment.Data.Entities
{
    public class RegionEntity : Entity
    {
        public static readonly string TableName = nameof(RegionEntity);

        public string Name { get; set; }
        public string ColorHex { get; set; }

        /// <remarks>Можно серилизовать и хранить как байт массив, но сейчас это не важно.</remarks>
        public string PointsJson { get; set; }
    }
}