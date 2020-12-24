using System;

namespace Apartment.Data.Entities
{
    public class ApartmentEntity : Entity
    {
        public static readonly string TableName = nameof(ApartmentEntity);

        /// <summary>
        /// Идентификатор сторонней системы.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Широта.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Долгота.
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// Цена.
        /// </summary>
        public int? Price { get; set; }

        /// <summary>
        /// Url объявления.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Номер квартиры.
        /// </summary>
        public int? ApartmentNumber { get; set; }

        /// <summary>
        /// Этаж.
        /// </summary>
        public int? Floor { get; set; }

        /// <summary>
        /// Количество этажей в здании.
        /// </summary>
        public int? FloorsCount { get; set; }

        /// <summary>
        /// Кол-во комнат.
        /// </summary>
        /// <remarks>NULL - свободная планировка; 0 - студия; >=1 - кол-во комнат.</remarks>
        public int? RoomsCount { get; set; }

        /// <summary>
        /// Площадь м².
        /// </summary>
        public double? Area { get; set; }

        /// <summary>
        /// Заголовок объявения.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Адрес.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Дата публикации объявления.
        /// </summary>
        // TODO: Переименовать в RaisedAtUtc.
        public DateTime PublishingDate { get; set; }

        // TODO: Добавить поле FirstMentionedAtUtc. Оно будет хранить в себе время первого поднятия/добавления объявления, которое мы заметили.

        /// <summary>
        /// Дата исчезновения объявления.
        /// </summary>
        public DateTime? DisappearedDate { get; set; }

        /// <summary>
        /// Json с коллекцией ссылок на изображения.
        /// </summary>
        public string ImageUrlsJson { get; set; }
    }
}