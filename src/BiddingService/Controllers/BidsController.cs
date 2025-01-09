using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using MassTransit.Futures.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController: ControllerBase
    {
        private readonly IMapper _mapper;

        //Access to mass transit to send notifications 
        private readonly IPublishEndpoint _publishEndpoint;

        //Access to gRPC for synchronose comunication to other services generated from .proto file 
        private readonly GrpcAuctionClient _grpcClient;

        public BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClient grpcClient )
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _grpcClient = grpcClient;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount) 
        {
            //try to find auction in bidding service database
            var auction = await DB.Find<Auction>().OneAsync(auctionId);
            if(auction == null)
            {
                //not found, then use gRPC (Google Remote Procedure Calls) to auction service and see if exists there
                auction = _grpcClient.GetAuction(auctionId);
                if (auction == null) return BadRequest("Can not accept bids on this auction at this time");
            }

            //Cannot bid on your auctions
            if(auction.Seller == User.Identity.Name)
            {
                return Forbid("You can not bid on your own auction");
            }

            var bid = new Bid 
            {
                Amount = amount,
                AuctionId = auctionId,
                Bidder = User.Identity.Name
            };

            if( auction.AuctionEnd < DateTime.UtcNow)
            {
                bid.BidStatus = BidStatus.Finished;
            }
            else
            {
                var highestBid = await DB.Find<Bid>()
                .Match(a=>a.AuctionId == auctionId)
                .Sort(x=>x.Descending(b => b.Amount))
                .ExecuteFirstAsync();
                if(highestBid != null && amount > highestBid.Amount || highestBid == null)
                {
                    bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve; 
                }

                if(highestBid != null && bid.Amount <= highestBid.Amount)
                {
                    bid.BidStatus = BidStatus.TooLow;
                }
            }
            
            var bidPlaced = _mapper.Map<BidPlaced>(bid);
            //bidPlaced.Id = Guid.NewGuid().ToString();
            
            
            await DB.SaveAsync(bid);

            //send a message to update currentHighBid in auction and search services
            await _publishEndpoint.Publish(bidPlaced);

            return Ok(bidPlaced);
        }
        
        [HttpGet("{auctionId}")]
        public async  Task<ActionResult<List<BidDto>>> GetBids(string auctionId)
        {
            var bids = await DB.Find<Bid>()
            .Match(a=>a.AuctionId == auctionId)
            .Sort(b=>b.Descending(a => a.BidTime))
            .ExecuteAsync();


            // return _mapper.Map<List<BidDto>>(bids);
            //project to BidDto and return it
            return bids.Select(_mapper.Map<BidDto>).ToList();
        }
    }
}