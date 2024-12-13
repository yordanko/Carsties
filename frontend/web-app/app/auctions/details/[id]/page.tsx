// use frc to generate empty shell to start with your page
// Note: To have routing functionality work page.tsx must be the name of a http page that we can route to! 
// Name of the component does not matter.
import { getDetailedViewData } from '@/app/actions/auctionAction'
import Heading from '@/app/components/Heading';
import React from 'react'
import CountdownTimer from '../../CountdownTimer';
import CarImage from '../../CarImage';
import DetailedSpecs from './DetailedSpec';
import { getCurrentUser } from '@/app/actions/authActions';
import EditButton from './EditButton';
import DeleteButton from './DeleteButton';


// Note: using of async in Details component. Use it only on server side component!
export default async function Details({params}:{params:{id:string}}) {

  const data = await getDetailedViewData(params.id);
 
   const user = await getCurrentUser();
  return (
    <>
    <div className='flex justify-between'>
      <div className='flex items-center gap-3'>
          <Heading title={`${data.make} ${data.model}`} />
          {user?.username === data.seller && (
            <>
              <EditButton id={data.id} />
              <DeleteButton id={data.id} />
            </>
          )}
      </div>
      
      <div className='flex gap-3'>
        <h3 className='text-2xl font-semibold'>Time Remaining</h3> 
        <CountdownTimer auctionEnd={data.auctionEnd}/>
      </div>
    </div>
    <div className='grid grid-cols-2 gap-6 mt-3'>
      <div className="relative aspect-[4/3] rounded-lg overflow-hidden">
      <CarImage auction={data}/>
      </div>
      <div className='border-2 rounded-lg p-2 bg-gray-100'>
        <Heading title='Bids' />
      </div>
    </div>
    <div className='mt-3 grid grid-cols-1 rounded-lg'>
      <DetailedSpecs auction={data} />
    </div>
      
    </>
  )
}
