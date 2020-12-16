namespace Apartment.Data.Entities
{
    public class RegionEntity : Entity
    {
        public static readonly string TableName = nameof(RegionEntity);

        public string Name { get; set; }
        //TODO : Заменить на строку формата #FF00FF. Получить цвет можно так: ColorTranslator.FromHtml(...), но там не тот color, что нужен для WPF
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }

        /// <remarks>Можно серилизовать и хранить как байт массив, но сейчас это не важно.</remarks>
        public string PointsJson { get; set; }
    }
}