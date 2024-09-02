import React, { useState, useContext, useEffect } from 'react'
import Task from './Task'
import { Stack } from '@mui/material'
import AddIcon from '@mui/icons-material/Add';
import ChipButton from './ChipButton';
import Navbar from './Navbar';
import PrimeButton from './PrimeButton';
import SecButton from './SecButton';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { UserContext } from './UserHook';
import { baseURL } from '../Utils';
import { getLocalStorage, setLocalStorage } from '../utils/functions';

export default function TaskBoard(props) {
    const { propUserData, parseUserData } = props;
    const [userData, setUserData] = useState(null);
    const navigate = useNavigate();
    const [tasksBefore, setTasksBefore] = useState([]);
    const [tasksAfter, setTasksAfter] = useState([]);
    const [selectedOption, setSelectedOption] = useState();
    const location = useLocation();
    let hasChildren = false;
    const [filteredCategories, setFilteredCategories] = useState([]);
    const [selectedCategories, setSelectedCategories] = useState([]);
    const url = baseURL();
    const allCategories = [
        { id: 1, name: 'בעלי חיים' },
        { id: 2, name: 'טיסה' },
        { id: 3, name: 'עבודה' },
        { id: 4, name: 'בריאות' },
        { id: 5, name: 'מגורים' },
        { id: 6, name: 'פנאי' },
        { id: 7, name: 'חינוך ילדים' },
        { id: 8, name: 'הובלה' },
        { id: 9, name: 'חינוך בוגרים' },
        { id: 10, name: 'ביטוחים' },
        { id: 11, name: 'רכב' },
        { id: 12, name: 'קהילות' }
    ]
    const userId = getLocalStorage("currentUser");
    console.log(userId);
    useEffect(() => {
        const localStorageData = { HasChildren: getLocalStorage(userId).have_kids, SelectedCategories: getLocalStorage(userId).category_active }
        propUserData ? setUserData(propUserData) : setUserData(localStorageData);
        hasChildren = localStorageData.HasChildren;
        setSelectedCategories(localStorageData.SelectedCategories);
    }, [])

    useEffect(() => {
        const filtered = allCategories.filter(cat => selectedCategories.includes(cat.id));
        setFilteredCategories(filtered);
    }, [selectedCategories])

    useEffect(() => {
        if (filteredCategories.length > 0 && !selectedOption) {
            setSelectedOption(filteredCategories[0].id)
        }
    }, [filteredCategories, selectedOption])


    useEffect(() => {
        if (selectedOption && userId) {
            fetchTasks(selectedOption);
        }
    }, [selectedOption, userId]);

    const addNewTask = () => {
        navigate('/edit-task/${userTaskId}', { state: { task: { recommendedTask: "כותרת משימה", descriptionTask: "תיאור משימה" }, categoryId: selectedOption, newTask: true } });
    };

    const handleButton = (categoryId) => {
        console.log("changing:", categoryId);
        setSelectedOption(categoryId);
    }

    const fetchTasks = (selectedOption) => {
        console.log("Fetching tasks for category:", selectedOption, " with children: ", hasChildren);
        const requestOptions = {
            method: "GET",
            redirect: "follow"
        };

        fetch(`${url}UserTasks/tasks/user/${userId}/final`, requestOptions)
            .then((response) => response.json())
            .then((result) => {
                console.log(result)
                const releventTasks = result.filter(task => {
                    const categoryMatch = task.categoryId === selectedOption;
                    const notDeleted = !task.IsDeleted;
                    return categoryMatch && notDeleted;
                });

                setTasksBefore(releventTasks.filter(task => task.isBeforeMove));
                console.log(releventTasks.filter(task => task.isBeforeMove));
                setTasksAfter(releventTasks.filter(task => !task.isBeforeMove));
                console.log(releventTasks.filter(task => !task.isBeforeMove))
            })
            .catch((error) => console.error(error));
    }

    const deleteTask = (userTaskId, userId) => {
        const myHeaders = new Headers();
        myHeaders.append("Content-Type", "application/json");

        const raw = {
            "userTaskId": userTaskId,
            "IsDelete": true
        };

        const requestOptions = {
            method: "DELETE",
            headers: myHeaders,
            body: JSON.stringify(raw),
        };

        fetch(`${url}UserTasks/${userId}/usertask/${userTaskId}`, requestOptions)
            .then((response) => response.json())
            .then((result) => {
                console.log("task deleted:", result)
                setTasksBefore(prevTasks => prevTasks.filter(task => task.userTaskId !== userTaskId));
                setTasksAfter(prevTasks => prevTasks.filter(task => task.userTaskId !== userTaskId));
            })
            .catch((error) => console.error(error));
    }

    const handleTaskClick = (task) => {
        console.log(task)
        navigate('/edit-task/${userTaskId}', { state: { task: task, categoryId: selectedOption } });
    }

    const handleNext = () => {
        const user = getLocalStorage(userId)
        user.completeReg = true;
        setLocalStorage(userId, user);
        const allRemainingTasks = [...tasksBefore, ...tasksAfter];
        setLocalStorage('allRemainingTasks', allRemainingTasks)
        navigate('/home', { state: { tasks: allRemainingTasks } });
    }
    const fromCategories = props.fromCategories || false;
    const fromEditTask = location.state?.fromEditTask || false;
    const showBackAndDots = fromCategories || fromEditTask;

    return (
        <div className='taskboard-container' >
            {showBackAndDots && <div className='stepIndicator' dir='rtl' >
                <div className='dot'></div>
                <div className='dot'></div>
                <div className='dot'></div>
                <div className='dot active'></div>
            </div>}
            <div style={{ display: 'flex', justifyContent: 'center' }}>
                <h4 style={{ textAlign: 'center' }}>בניית לוח משימות</h4>
            </div>
            <div className='chip-container' style={{ maxWidth: '393px', overflowX: 'scroll', whiteSpace: 'nowrap' }}>
                <Stack direction="row-reverse" spacing={1} style={{ flexWrap: 'nowrap', overflowX: 'scroll', maxWidth: '310px' }} >
                    {filteredCategories.map(category => (
                        <ChipButton
                            key={category.id}
                            txt={category.name}
                            onClick={() => handleButton(category.id)} active={selectedOption === category.id}
                        />
                    ))}
                </Stack>
            </div>
            <div className='taskrec'>
                <h3 style={{ width: '100%', fontSize: '18px', textAlign: 'right', fontWeight: '300', fontWeight: 'bold' }}>לפני המעבר</h3>
                {tasksBefore.map((task, index) => (
                    <Task onClick={() => handleTaskClick(task)}
                        key={`${task.taskId}-${index}`}
                        date={(task.startDate)}
                        label={task.taskName}
                        description={task.taskDescription
                        }
                        onDelete={() => deleteTask(task.userTaskId, userId)}
                    />
                ))}
            </div>
            <div className='taskrec'>
                <h3 style={{ fontSize: '18px', float: 'right', fontWeight: '300', fontWeight: 'bold' }}>אחרי המעבר</h3>
                {tasksAfter.map((task, index) => (
                    <Task onClick={() => handleTaskClick(task)}
                        key={`${task.taskId}-${index}`}
                        date={task.startDate}
                        label={task.taskName}
                        description={task.taskDescription}
                        onDelete={() => deleteTask(task.userTaskId, userId)} />
                ))}
            </div>
            <Stack spacing={1} direction='column' sx={{ width: '70%', margin: 'auto' }}>
                <SecButton btntxt="הוספת משימה חדשה" onClick={addNewTask} >
                    {<AddIcon />}
                </SecButton>
                <PrimeButton onClick={handleNext} btntxt="הבא" />
            </Stack>
            {!showBackAndDots ? <Navbar /> : null}
        </div>

    )
}