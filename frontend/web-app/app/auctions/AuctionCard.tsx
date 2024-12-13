'use client'
import React from 'react'
import CountdownTimer from './CountdownTimer'
import CarImage from './CarImage'
import { Auction } from '@/types'
import Link from 'next/link'

type Props = {
    auction: Auction
}   

export default function AuctionCard({auction}: Props) {
  return (
     <Link href={`auctions/details/${auction.id}`} className='group'>
        <div className='relative w-full bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden'>
            <CarImage auction={auction}></CarImage>
            <div className='absolute bottom-2 left-2'>
            <CountdownTimer auctionEnd={auction.auctionEnd}/>
            </div>
        </div>
        <div className='flex justify-between items-center mt-4 text-sm'>
            <h3 className='text-gray-700 '>{auction.make} {auction.model}</h3>
            <p className='font-semibold'>{auction.year}</p>
        </div>
        
    </Link> 
    
  )
}
