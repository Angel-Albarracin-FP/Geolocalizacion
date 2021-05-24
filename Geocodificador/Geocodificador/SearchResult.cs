namespace Geocodificador
{
    public class SearchResult
    {
        public Address address { get; set; }
        public string[] boundingbox { get; set; }
        public string _class { get; set; }
        public string display_name { get; set; }
        public float importance { get; set; }
        public string lat { get; set; }
        public string licence { get; set; }
        public string lon { get; set; }
        public int osm_id { get; set; }
        public string osm_type { get; set; }
        public int place_id { get; set; }
        public string svg { get; set; }
        public string type { get; set; }
    }

    public class Address
    {
        public string city { get; set; }
        public string city_district { get; set; }
        public string construction { get; set; }
        public string continent { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string house_number { get; set; }
        public string neighbourhood { get; set; }
        public string postcode { get; set; }
        public string public_building { get; set; }
        public string state { get; set; }
        public string suburb { get; set; }
    }
}