import React from 'react'
import './Realocation.css';

function CategoryItem({ image, label, onClick, active }) {

  return (
    <div onClick={onClick} style={{
      backgroundColor: active ? '#0C8CE9' : 'white',
      border: '1px solid #E7EFFA',
      borderRadius: '8px',
      padding: '27px 35px',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      margin: '8px',
      height: '72px',
      width: '30px'
    }}>
      <img src={image} alt={label} className='card-image' />
      <p className='card-label'>{label}</p>
    </div>
  );
}

export default CategoryItem;