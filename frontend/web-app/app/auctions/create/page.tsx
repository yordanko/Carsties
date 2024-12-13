// use frc to generate empty shell to start with your page
// Note: To have routing functionality work page.tsx must be the name of a http page that we can route to! 
// Name of the component does not matter.
import Heading from '@/app/components/Heading'
import React from 'react'
import AuctionForm from '../AuctionForm'

export default function Create() {
  return (
    <div className='mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg'>
      <Heading title='Sell your car!' subtitle='Please enter details of your car'/>
      <AuctionForm />
    </div>
  )
}
