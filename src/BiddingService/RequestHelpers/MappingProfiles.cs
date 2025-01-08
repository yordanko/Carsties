using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using Contracts;

namespace BiddingService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bid, BidDto>().ForMember(x=>x.BidStatus, o=>o.MapFrom(src=>src.BidStatus.ToString()));
            CreateMap<Bid, BidPlaced>();
        }
        
    }
}