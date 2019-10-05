using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace FellowLibrary.DatabaseAccess
{
    public interface IDataContext
    {
        IEnumerable<Models.TradeEntry> GetTrades();
        void InsertTrades(IEnumerable<Models.TradeEntry> items);
    }

    public sealed class DatabaseContext
    {
        public DatabaseContext(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            this.Database = client.GetDatabase(settings.DatabaseName);
        }

        IMongoDatabase Database { get; set; }

        public IMongoCollection<Models.TradeEntry> Trades { get { return this.Database.GetCollection<Models.TradeEntry>("TradeEntry"); } }

        
    }
}
