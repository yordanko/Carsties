//rfc  - use to create shell

'use client'
import { useParamsStore } from '@/hooks/useParamsStore'
import { usePathname } from 'next/navigation'
import { useRouter } from 'next/navigation'
import React from 'react'
import { IoCarSportOutline } from 'react-icons/io5'

export default function Logo() {

    //NOTE: get only reset function from store
  const reset = useParamsStore(state => state.reset)
  const router = useRouter();
  const pathname = usePathname();
  function doReset(){
    if(pathname !== '/')
      router.push('/')
    reset();
  }
   
  return (
      <div onClick={doReset} className='flex items-center gap-2 text-3xl font-semibold text-red-500 cursor-pointer'>
          <IoCarSportOutline size={34}/>
          <div>Carsties Auctions</div>
        </div>
    )
}
