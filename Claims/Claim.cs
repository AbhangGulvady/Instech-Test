using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims
{
    public class Claim
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("coverId")]
        public string CoverId { get; set; }


        //Here, had to remove DateOnly(true) attribute due to swagger giving 500 error.
        //alternatively, we can use this below code keeping DateOnly(true) 
        //cover.StartDate = cover.StartDate.Date;
        //cover.EndDate = cover.EndDate.Date;
        //but dont want to add boiler plate code here.
        [BsonElement("created")]
        public DateTime Created { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("claimType")]
        public ClaimType Type { get; set; }

        [BsonElement("damageCost")]
        public decimal DamageCost { get; set; }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
