import React, { useState } from 'react'
import './App.css'
import RealocationApp from './Componemts/RealocationApp'
import { UserContext, UserHook  } from './Componemts/UserHook'


function App() {
  const {userDetails, setUserDetails} = UserHook()
  return (
    <>
      <div>
        <UserContext.Provider value={{userDetails, setUserDetails}} >
         <RealocationApp />
         </UserContext.Provider >
      </div>
    </>
  )
}

export default App
