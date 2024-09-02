import { Switch, IconButton, TextField } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import React, { useState } from 'react';
import CircleButton from './CircleButton';
import SecButton from './SecButton';
import PrimeButton from './PrimeButton';
import { useLocation, useNavigate } from 'react-router-dom';
import { baseURL } from '../Utils';
import { getLocalStorage } from '../utils/functions';


export default function EditTask(props) {
  const { parseUserData, userData } = props;
  const { state } = useLocation();
  const taskFromState = state.task;
  const is_newTask = state.newTask;
  const categoryId = state.categoryId;
  const [task, setTask] = useState(taskFromState);
  const [active, setActive] = useState(task ? task.priority : null);
  const navigate = useNavigate();
  const [isOneDayTask, setIsOneDayTask] = useState(false);
  const url = baseURL();
  const userId = getLocalStorage("currentUser");
  const [title, setTitle] = useState(task.taskName || "כותרת משימה");
  const [description, setDescription] = useState(task.taskDescription || "תיאור משימה");
  const [isEditingTitle, setIsEditingTitle] = useState(false);
  const [isEditingDesc, setIsEditingDesc] = useState(false);

  const formatDateForInput = (date) => {
    if (!date) return '';
    const d = new Date(date);
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    const year = d.getFullYear();
    return `${year}-${month}-${day}`;
  };

  const handleTitleClick = () => {
    setIsEditingTitle(true);
  };

  const handleDescClick = () => {
    setIsEditingDesc(true);
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setTask((prevTask) => ({
      ...prevTask,
      [name]: value,
    }));
    console.log(task.taskName)
    if (name === "taskName") {
      setTitle(value);
    } else if (name === "taskDescription") {
      setDescription(value);
    }
  };

  const toggleActive = (label) => {
    let newPriority;
    if (label === 'דחוף') {
      newPriority = 3;
    } else if (label === 'חשוב') {
      newPriority = 2;
    } else if (label === 'כדאי') {
      newPriority = 1;
    }

    setActive(active === label ? null : label);
    setTask(prevTask => ({
      ...prevTask,
      priority: active === label ? null : newPriority
    }));
  };

  const handleBlur = () => {
    setIsEditingTitle(false);
    setIsEditingDesc(false);
  };

  const handleStartDateChange = (e) => {
    const { value } = e.target;
    setTask(prevTask => ({
      ...prevTask,
      startDate: value,
      endDate: isOneDayTask ? value : prevTask.endDate 
    }));
  };

  const handleEndDateChange = (e) => {
    const { value } = e.target;
    setIsOneDayTask(false); 
    setTask(prevTask => ({
      ...prevTask,
      endDate: value
    }));
  };

  const toggleOneDayTask = () => {
    setIsOneDayTask(!isOneDayTask);
    if (!isOneDayTask) {
      setTask(prevTask => ({
        ...prevTask,
        endDate: prevTask.startDate
      }))
    }
  }

  const updateTask = () => {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const raw = JSON.stringify({
      "TaskName": task.taskName,
      "TaskDescription": task.taskDescription,
      "PriorityId": task.priority,
      "StartDate": task.startDate,
      "EndDate": task.endDate,
      "PersonalNote": task.personalNote
    });
    console.log(task);

    const requestOptions = {
      method: "PUT",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };

    fetch(`${url}UpdateTask/tasks/update/${task.userTaskId}`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        console.log(result);
        navigate('/tasks-board', { state: { fromEditTask: true } })
      })
      .catch((error) => console.error(error));
  }

  const newTask = () => {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    console.log("%%", categoryId)
    const raw = JSON.stringify({
      "UserId": userId,
      "TaskName": task.taskName || "שם משימה",
      "TaskDescription": task.taskDescription || "תיאור משימה",
      "StartDate": task.startDate,
      "EndDate": task.endDate,
      "PriorityId": task.priority,
      "PersonalNote": task.personalNote,
      "categoryId": categoryId
    });

    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };

    fetch(`${url}UserTasks/tasks/new`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        console.log("new task:", result)
        navigate('/tasks-board', { state: { fromEditTask: true } });
      })
      .catch((error) => console.error(error));
  }

  return (
    <div className='edit-container'>
      <div style={{ borderBottom: '1px solid #b3ccef', padding: '8px' }}>
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: '0 8px' }}>
          <IconButton onClick={() => navigate('/tasks-board', { state: { fromEditTask: true } })} style={{ transform: 'scaleX(-1)', position: 'absolute', left: '330px' }}>
            <ArrowBackIcon />
          </IconButton>
          {isEditingTitle ? (
            <input
              type="text"
              name="taskName"
              value={task.taskName}
              onChange={handleInputChange}
              onBlur={handleBlur}
              autoFocus
              style={{ width: '30%' }}
            />
          ) : (
            <h4 style={{ color: '#0C8CE9', maxWidth: '260px', margin: '0 auto', width: '100%', textAlign: 'center' }} onClick={handleTitleClick}>{title}</h4>
          )}
        </div>
        <div>
          {isEditingDesc ? (
            <input
              type="text"
              name="taskDescription"
              value={task.taskDescription}
              onChange={handleInputChange}
              onBlur={handleBlur}
              autoFocus
            />
          ) : (
            <p style={{ color: '#0C8CE9', maxWidth: '300px', margin: '0 auto', width: '100%', textAlign: 'center' }} onClick={handleDescClick}>{description}</p>
          )}
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Switch checked={isOneDayTask} onChange={toggleOneDayTask} />
          <span>
            יום אחד
          </span>
        </div>
        <button>
          <div style={{ width: '250px', height: '35px', border: '1px solid #b3ccef', borderRadius: '8px', padding: '12px 16px', display: 'flex', justifyContent: 'space-between' }}>
            <div style={{ fontWeight: 'bold' }}>
              <input
                type="date"
                name="startDate"
                value={formatDateForInput(task.startDate)}
                onChange={handleStartDateChange}
              />
            </div>
            <div>התחלה</div>
          </div>
        </button>
        <button>
          <div style={{ width: '250px', height: '35px', border: '1px solid #b3ccef', borderRadius: '8px', padding: '12px 16px', display: 'flex', justifyContent: 'space-between' }}>
            <div style={{ fontWeight: 'bold' }}>
              <input
                type="date"
                name="endDate"
                value={formatDateForInput(task.endDate)}
                onChange={handleEndDateChange}
              />
            </div>
            <div>סיום</div>
          </div>
        </button>
      </div>
      <div style={{ borderBottom: '1px solid #b3ccef', padding: '8px' }}>
        <p style={{ textAlign: 'right' }}>:דחיפות משימה</p>
        <div style={{ display: 'flex', flexDirection: 'row-reverse', justifyContent: 'flex-start', gap: '8px', alignItems: 'center' }}>
          <CircleButton color="#e55c5c" label="דחוף" onClick={toggleActive} active={active === 'דחוף' || active === 3} />
          <CircleButton color="#e5e05c" label="חשוב" onClick={toggleActive} active={active === 'חשוב' || active === 2} />
          <CircleButton color="#67e55c" label="כדאי" onClick={toggleActive} active={active === 'כדאי' || active === 1} />
        </div>
      </div>
      <div style={{ marginTop: '16px' }}>
        <p style={{ display: 'flex', justifyContent: 'flex-end' }}>הערות אישיות</p>
        <TextField
          name="personalNote"
          fullWidth
          multiline
          rows={8}
          placeholder="הערות אישיות"
          variant="outlined"
          value={task.personalNote}
          onChange={handleInputChange}
          style={{ margin: '0px', direction: 'rtl', marginBottom: '16px' }} />
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
        {!is_newTask && <PrimeButton onClick={updateTask} btntxt="שמירה" />}
        <SecButton onClick={newTask} btntxt="הוספת משימה חדשה" />
      </div>
    </div>
  )
}
