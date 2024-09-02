import React from 'react'

export default function PrimeButton({ btntxt, onClick, disabled }) {

  return (

    <button onClick={() => { onClick() }} disabled={disabled} style={{
      color: 'white',
      backgroundColor: disabled ? '#cccccc' : '#0C8CE9',
      padding: '8px 54px',
      fontSize: '18px',
      borderRadius: '50px'
    }}>
      {btntxt}
    </button>
  )
}
