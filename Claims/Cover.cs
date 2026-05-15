using DotNet.Testcontainers.Builders;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims;

public class Cover
{
    [BsonId]
    public string Id { get; set; }

    //Here, had to remove DateOnly(true) attribute due to swagger giving 500 error.
    //alternatively, we can use this below code keeping DateOnly(true) 
    //cover.StartDate = cover.StartDate.Date;
    //cover.EndDate = cover.EndDate.Date;
    //but dont want to add boiler plate code here.
    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}
