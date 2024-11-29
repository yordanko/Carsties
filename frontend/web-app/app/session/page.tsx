//Use rfc snippet to get initial component
import { auth } from '@/auth';
import React from 'react'
import Heading from '../components/Heading';
import AuthTest from './AuthTest';

export default async function Session() {
    const session = await auth();
    return (
        <div>
            <Heading title='Session dashboard' subtitle='Session data'/>
            <div className="bg-blue-200 border-2 border-blue-500">
                
                <pre className='whitespace-pre-wrap break-all'>{JSON.stringify(session, null, 2)}</pre>
            </div>
            <div className='mt-4'>
                <AuthTest/>
            </div>
        </div>
    )
}
