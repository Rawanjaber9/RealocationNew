import React from 'react'

export default function SecButton({ btntxt, onClick, active }) {
  return (
    <button onClick={onClick} style={{
      backgroundColor: active ? '#0C8CE9' : 'transparent',
      color: active ? 'white' : '#0C8CE9',
      border: '1px solid #0C8CE9',
      padding: '10px 32px',
      fontSize: '14px',
      borderRadius: '50px',
    }}>
      {btntxt}
    </button>
  )
}
