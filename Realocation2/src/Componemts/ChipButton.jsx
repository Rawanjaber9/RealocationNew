import React from 'react'

export default function ChipButton({ txt, onClick, active }) {

  return (
    <div>
      <button onClick={onClick} style={{
        fontSize: '14px',
        padding: '6px 14px',
        border: '1px solid',
        borderRadius: '50px',
        backgroundColor: active ? '#1170f4' : 'transparent',
        color: active ? 'white' : '#1170f4'
      }}>
        {txt}
      </button>
    </div>
  )
}
