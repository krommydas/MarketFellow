using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FellowLibrary.Models
{
    public sealed class TradeEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String ID { get; set; }

        public int Provider { get; set; }

        public String TradingPair { get; set; }

        //public TradingPair To { get; set; }

        public Decimal Price { get; set; }

        public DateTime DateTime { get; set; }
    }
}
