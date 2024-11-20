//rfc  - use to create shell

'use client'
import { useParamsStore } from '@/hooks/useParamsStore'
import React from 'react'
import { IoCarSportOutline } from 'react-icons/io5'

export default function Logo() {

    //NOTE: get only reset function from store
    const reset = useParamsStore(state => state.reset)

    function onClick() {

    }
  return (
    <div onClick={reset} className='flex items-center gap-2 text-3xl font-semibold text-red-500 cursor-pointer'>
        <IoCarSportOutline size={34}/>
        <div>Carsties Auctions</div>
      </div>
  )
}
