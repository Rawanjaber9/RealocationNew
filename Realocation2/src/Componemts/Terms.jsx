import React, { useEffect, useState, useContext } from 'react'
import { useLocation, useNavigate } from 'react-router-dom';
import PrimeButton from './PrimeButton';
import Navbar from './Navbar';
import { IconButton } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { baseURL } from '../Utils';
import { UserContext } from './UserHook';
import OpeningQuestions from './OpeningQuestions';
import Categories from './Categories';
import TaskBoard from './TaskBoard';
import EditTask from './EditTask';

export default function Terms() {
  const [isAccepted, setIsAccepted] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const url = baseURL();
  const { userDetails, setUserDetails } = useContext(UserContext);
  const [PageName, setPageName] = useState('Terms')
  const fromReg = location.state?.fromReg;
  const [userData, setUserData] = useState({})

  const parseUserData = (obj, PageName) => {
    let data = userData
    const arr = Object.keys(obj)
    for (let i = 0; i < arr.length; i++) {
      const key = arr[i];
      data[key] = obj[key];
    }
    console.log(data);
    setUserData(data);
    setPageName(PageName);
  }

  useEffect(() => {
    if (!userDetails) {
    }
    else {
      const requestOptions = {
        method: "GET",
        redirect: "follow"
      };

      fetch(`${url}register/${userDetails.userId}`, requestOptions)
        .then((response) => response.json())
        .then((result) => {
          console.log(result);
          setIsAccepted(result.hasAcceptedTerms)
        })
        .catch((error) => console.error(error));
    }
  }, [])


  const acceptTerms = () => {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const raw = JSON.stringify(isAccepted);

    const requestOptions = {
      method: "PUT",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };
    setPageName('opningQuestions')
    fetch(`${url}register/accept-terms/${userDetails.userId}`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        setUserDetails({ userId: result.userId });
        setPageName('opningQuestions')
      })
      .catch((error) => {
        console.log(error);
      });
  }
  if (PageName === 'opningQuestions') {
    return (
      <>
        <OpeningQuestions userId={userDetails.userId} parseUserData={parseUserData} />
      </>
    )
  }
  else if (PageName === "cetegories") {
    return (
      <>
        <Categories userId={userDetails.userId} parseUserData={parseUserData} userData={userData} />
      </>
    )
  }
  else if (PageName === "taskBoard") {
    return (
      <>
        <TaskBoard userId={userDetails.userId} userData={userData} parseUserData={parseUserData} fromCategories={true} />
      </>
    )
  }
  else if (PageName === "editTask") {
    return (
      <>
        <EditTask userId={userDetails.userId} userData={userData} />
      </>
    )
  }
  else {
    return (
      <div style={{ padding: '24px' }}>
        {fromReg && <div className='stepIndicator' dir='rtl' >
          <div className='dot active'></div>
          <div className='dot'></div>
          <div className='dot'></div>
          <div className='dot'></div>
        </div>}
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', width: '100%' }}>
          {fromReg && <IconButton onClick={() => navigate(-1)} style={{ transform: 'scaleX(-1)', position: 'absolute', left: '320px' }}>
            <ArrowBackIcon />
          </IconButton>}
          <h4 style={{ textAlign: 'center' }}>תנאי שימוש</h4>
        </div>
        <p style={{ marginBottom: '24px', textAlign: 'right', direction: 'rtl' }}>
          ברוכים הבאים לאליקציית Realocation. אפליקציה זו נועדה לעזור למשתמשים להתמודד עם תהליך המעבר, תוך הצעת משימות אורגניזטוריות לניהול התהליך.
        </p>
        <p style={{ marginBottom: '24px', textAlign: 'right', direction: 'rtl' }}>
          האפליקציה אינה מחויבת לעדכון חוקי או מקצועי ומספקת כלים תומכים בלבד.
        </p>
        <p style={{ marginBottom: '120px', textAlign: 'right', direction: 'rtl' }}>
          השימוש באפליקציה מוגבל למטרות אישיות והמידע המועבר דרכה אינו לשימוש מסחרי או הפצה נוספת. פרטיות המשתתפים מוגנת והמידע אודותיהם לא ישותף ללא הסכמתם.
        </p>
        <div style={{ textAlign: 'right' }}>
          {fromReg && <label htmlFor="">
            אני מסכימ.ה לתנאי השימוש
            <input
              type='checkbox'
              checked={isAccepted}
              onChange={e => setIsAccepted(e.target.checked)}
              style={{ marginLeft: '8px' }}
            />
          </label>
          }</div>

        <div style={{ marginTop: '32px' }}>
          {fromReg && <PrimeButton onClick={acceptTerms} btntxt="הבא" disabled={!isAccepted} />}</div>
        {!fromReg && <Navbar />}
      </div>
    )
  }
}