import React from 'react';
import SecButton from './SecButton';
import { setLocalStorage } from '../utils/functions';

export default function HomeTask({ tasks, setTasks, filterTasks }) {
    const handleComplete = (taskId) => {
        const newTasks = tasks.map(task => {
            if (taskId === task.userTaskId) {
                return { ...task, completed: !task.completed }
            }
            else {
                return task
            }
        })
        setTasks(newTasks)
        setLocalStorage('allRemainingTasks', tasks)
    };

    const urgencyColor = (urgency) => {
        switch (urgency) {
            case 1: return '#E55C5C';
            case 2: return '#E5E05C';
            case 3: return '#67E55C';
            default: return 'grey';
        }
    };

    const formatDateForDisplay = (dateString) => {
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        return `${day}/${month}`;
    };

    return (
        <>
            {filterTasks().map((task) => {
                console.log("***", task)
                return (
                    <div key={task.id} style={{
                        padding: '16px 32px',
                        backgroundColor: task.completed ? '#D9E4F4' : 'white',
                        borderRadius: '16px',
                        marginBottom: '16px',
                        border: '1px solid #E7EFFA',
                        direction: 'rtl',
                        width: '80%',
                        margin: '0 auto',
                        position: 'relative',
                        overflow: 'hidden',
                        marginTop: '16px'
                    }}>
                        <div style={{
                            height: '100%',
                            width: '16px',
                            backgroundColor: urgencyColor(task.priority),
                            borderRadius: '0 16px 16px 0',
                            position: 'absolute',
                            right: '0',
                            top: '0'
                        }} />
                        <div style={{ textAlign: 'right' }}>
                            <h4 style={{ fontSize: '18px', fontWeight: 'bold', color: task.completed ? 'gray' : '#0C8CE9', margin: '0' }}>{task.taskName}</h4>
                            <p style={{ fontSize: '14px', color: task.completed ? 'gray' : '#0C8CE9', margin: '0' }}>{task.taskDescription}</p>
                            <p style={{ fontSize: '14px' }}> עד {formatDateForDisplay(task.endDate)}</p>
                            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px', marginTop: '10px' }} >
                                <SecButton onClick={() => handleComplete(task.userTaskId)} btntxt={task.completed ? "שחזר" : "בוצע"} active={!task.completed} />
                            </div>
                        </div>
                    </div>
                )
            })}
        </>
    )
}
