'use client'
import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResult } from '@/types';
import AppPagination from '../components/AppPagination';
import { getData } from '../actions/auctionAction';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/useParamsStore';
import { useShallow } from 'zustand/react/shallow';
import qs from 'query-string';
import { EmptyFilter } from '../components/EmptyFilter';


export default function Listings() {
  // const [auctions, setAuctions] = useState<Auction[]>([])
  // const [pageCount, setPageCount] = useState(0);
  // const [pageNumber, setPageNumber] = useState(1);
  // const [pageSize, setPageSize] = useState(4);
  const [data, setData] = useState<PagedResult<Auction>>();

  //NOTE: useShallow 
  const params = useParamsStore(useShallow(state => ({
    pageNumber:state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy  
  })));
  const setParams = useParamsStore(state=>state.setParams)
  const url = qs.stringifyUrl({url:'', query:params})

  function setPageNumber(pageNumber:number){
    setParams({pageNumber})
  }

  useEffect(()=>{
    getData(url).then(data => {
      setData(data)
    });
  },[url])

  if(!data) return (<>Loading...</>)
  return (
    <>
    <Filters />
      {data.totalCount ===0 ? (
        <EmptyFilter showReset/>
      ) : (
        <> 
          <div className='grid grid-cols-4 gap-6  text-sm'>
            {data.results.map((auction) => (
              <AuctionCard auction={auction} key={auction.id}/> 
            ) )}
          </div>
          <div className='flex justify-center'>
            <AppPagination currentPage={params.pageNumber} pageCount={data.pageCount} pageChanged={setPageNumber}/>
          </div>
        </>
      )}
      
    </>
  )
}
