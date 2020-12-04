using Newtonsoft.Json;

namespace Apartment.DataProvider.Avito
{
    internal class AvitoApartmentsResponse
    {
        public Applybutton applyButton { get; set; }
        public Item[] items { get; set; }
        public int count { get; set; }
    }

    internal class Applybutton
    {
        public bool enabled { get; set; }
        public string title { get; set; }
    }

    internal class Item
    {
        public string type { get; set; }
        public string title { get; set; }
        public bool favorite { get; set; }
        public int itemId { get; set; }
        public string url { get; set; }
        public Image[] images { get; set; }
        public int imagesCount { get; set; }
        public bool hasVideo { get; set; }
        public Category category { get; set; }
        public Ext ext { get; set; }
        public int price { get; set; }
        public Location location { get; set; }
        public Coords coords { get; set; }
        public int time { get; set; }
        public Priceformatted priceFormatted { get; set; }
        public Geo geo { get; set; }
    }

    internal class Category
    {
        public int id { get; set; }
        public int rootId { get; set; }
        public string slug { get; set; }
        public bool compare { get; set; }
    }

    internal class Ext
    {
        public string offline_check_result { get; set; }
        public string bezopasnyi_prosmotr { get; set; }
        public string nomer_kvartiry { get; set; }
        public string rooms { get; set; }
        public string type { get; set; }
        public string house_type { get; set; }
        public string floors_count { get; set; }
        public string floor { get; set; }
        public string address { get; set; }
        public string area_live { get; set; }
        public string area_kitchen { get; set; }
        public string commission { get; set; }
        public string offer_type { get; set; }
        public string gruzovoi_lift { get; set; }
        public int[] parkovka { get; set; }
        public string passazhirskii_lift { get; set; }
        public int[] udobstva { get; set; }
        public int[] v_podezde { get; set; }
        public int[] vo_dvore { get; set; }
        public string sanuzel { get; set; }
        public string remont { get; set; }
        public string tip_sdelki { get; set; }
        public string status { get; set; }
        public int[] vid_iz_okon { get; set; }
        public string balkon_ili_lodzhiya { get; set; }
        public int god_postroiki { get; set; }
        public string area { get; set; }
        public Title title { get; set; }
        public string otdelka { get; set; }
        public string development_property_engine_id { get; set; }
        public string proektnaya_deklaratsiya { get; set; }
        public string ofitsialnyy_zastroyshchik { get; set; }
        public int tip_uchastiya { get; set; }
        public string korpus_stroenie { get; set; }
        public string nazvanie_obekta_nedvizhimosti { get; set; }
        public int korpus { get; set; }
        public int nazvanie_novostroyki { get; set; }
        public int[] tip_komnat { get; set; }
        public float vysota_potolkov { get; set; }
    }

    internal class Title
    {
        public string full { get; set; }
        public string plain { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }
    }

    internal class Location
    {
        public string name { get; set; }
        public string namePrepositional { get; set; }
    }

    internal class Coords
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int zoom { get; set; }
        public int precision { get; set; }
        public string address_user { get; set; }
    }

    internal class Priceformatted
    {
        public Title1 title { get; set; }
        public string titleDative { get; set; }
        public bool enabled { get; set; }
        public bool was_lowered { get; set; }
        public bool has_value { get; set; }

        [JsonProperty("string")]
        public string String { get; set; }

        public int value { get; set; }
        public string exponent { get; set; }
    }

    internal class Title1
    {
        public string full { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }
    }

    internal class Geo
    {
        public Georeference[] geoReferences { get; set; }
        public string formattedAddress { get; set; }
    }

    internal class Georeference
    {
        public string content { get; set; }
    }

    internal class Image
    {
        [JsonProperty("208x156")]
        public string SmallSize { get; set; }

        [JsonProperty("416x312")]
        public string BigSize { get; set; }
    }
}