import React, { useContext } from 'react'
import IconButton from '@mui/material/IconButton';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PrimeButton from './PrimeButton'
import { UserContext } from './UserHook';
import { useNavigate } from 'react-router-dom';

export default function RestorePassword() {
  const { userDetails } = useContext(UserContext);
  const navigate = useNavigate();
  const sendCode = () => {
    alert(`קוד אימות`);
  };

  return (
    <div style={{ height: '100vh', padding: '24px' }}>
      <div style={{ display: 'flex', alignItems: 'center', padding: ' 0 24px' }}>
        <IconButton onClick={() => navigate(-1)} style={{ transform: 'scaleX(-1)', left: '230px' }}>
          <ArrowBackIcon />
        </IconButton>
        <h4 style={{ textAlign: 'center' }}>אמת את זהותך</h4>
      </div>
      <p style={{ marginBottom: '24px', textAlign: 'right', direction: 'rtl' }}>
        על מנת לשמור על ביטחון חשבונך, אנחנו נשלח קוד אימות בעל 6 ספרות למייל:
        <br />
        <br />
        אם אין באפשרותך גישה לכתובת מייל זו, אנא צור קשר עם התמיכה.
      </p>
      <PrimeButton onClick={sendCode} btntxt="שלח קוד" />
    </div>
  )
}
