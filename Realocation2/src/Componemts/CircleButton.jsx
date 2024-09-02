import React from 'react'

export default function CircleButton({ color, label, onClick, active }) {
  return (
    <div onClick={() => onClick(label)} >
      <button style={{
        width: '32px',
        height: '32px',
        background: active ? color : 'transparent',
        border: `2px solid ${color}`,
        borderRadius: '50%'
      }}></button>
      <p>{label}</p>
    </div>
  )
}
