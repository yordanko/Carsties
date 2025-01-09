using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using Grpc.Core;
using MassTransit.Monitoring.Performance.StatsD;

namespace AuctionService.Services
{
    //This class is used in gRPC server call.
    //Note: This class derived from GrpcAuction.GrpcAuctionBase which was generated when buid from .proto file in project
    public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
    {
        private readonly AuctionDbContext _dbContext;

        public GrpcAuctionService(AuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Must override this method. This method is decribed in .proto file
        public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, 
            ServerCallContext context)
            {
                Console.WriteLine("==> Received Grpc request for auction");
                var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
                    ?? throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

                var response = new GrpcAuctionResponse()
                {
                    Auction = new GrpcAuctionModel
                    {
                        AuctionEnd = auction.AuctionEnd.ToString(),
                        Id = auction.Id.ToString(),
                        ReservedPrice = auction.ReservePrice,
                        Seller = auction.Seller
                    }
                };

                return response;
            }
    }
}