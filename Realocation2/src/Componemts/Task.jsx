import React from 'react'
import './Realocation.css';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import IconButton from '@mui/material/IconButton';

export default function Task(props) {
  const { date, label, description, onDelete, onClick } = props;

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    return `${day}/${month}`;
  };

  const handleDelete = (e, label) => {
    e.stopPropagation();
    onDelete(label);
  }
  return (
    <div className="task-container" onClick={onClick} >
      <IconButton aria-label="delete" size="small" className="task-delete-button" onClick={(e) => handleDelete(e, label)} >
        <DeleteOutlineIcon style={{ color: '#1170f4' }} />
      </IconButton>
      <div className="task-content">
        <h3 className="task-label">{label}</h3>
        <p className="task-description">{description}</p>
      </div>
      <p className="task-date">{formatDate(date)}</p>
    </div>
  )
}
