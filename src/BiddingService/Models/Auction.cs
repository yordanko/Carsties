using MongoDB.Entities;

namespace BiddingService.Models;

//Mongo Db entities, these have to derived from MongoDB.Entities.Entity class
public class Auction : Entity
{
    public DateTime AuctionEnd { get; set; }
    public string Seller { get; set; }
    public int ReservePrice { get; set; }   
    public bool Finished { get; set; }
}