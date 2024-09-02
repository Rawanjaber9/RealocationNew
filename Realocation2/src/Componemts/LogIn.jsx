import TextField from '@mui/material/TextField';
import './Realocation.css';
import Stack from '@mui/material/Stack';
import PrimeButton from './PrimeButton';
import { useNavigate } from 'react-router-dom';
import { useContext, useState } from 'react';
import { baseURL } from '../Utils';
import { UserContext } from './UserHook';
import { getLocalStorage, setLocalStorage } from '../utils/functions';

function LogIn() {
    const navigate = useNavigate();
    const { setUserDetails } = useContext(UserContext);
    const [user, setUser] = useState();
    const [userExistsMSG, setUserExistsMSG] = useState('');
    const [password, setPassword] = useState();
    const url = baseURL();

    const btnlogin = () => {
        const myHeaders = new Headers();
        myHeaders.append("Content-Type", "application/json");

        const raw = JSON.stringify({
            "Username": user,
            "Password": password
        });

        const requestOptions = {
            method: "POST",
            headers: myHeaders,
            body: raw
        };
        fetch(`${url}login`, requestOptions)
            .then((response) => response.json())
            .then((result) => {
                console.log('login', result)
                setUserDetails({ userId: result.userId });
                let user = getLocalStorage(result.userId);
                if (!user) {
                    setLocalStorage(result.userId, { category_active: [], have_kids: "no", moveDate: { year: '', month: '', day: '' }, selected_country: { label: '' }, completeReg: false })
                    user = getLocalStorage(result.userId);
                }
                setLocalStorage("currentUser", result.userId)
                if (user.completeReg) {
                    navigate('/home');
                }
                else {
                    navigate('/terms', { state: { fromReg: true } });
                }
            })
            .catch((error) => {
                console.log(error)
                setUserExistsMSG("משתמש לא קיים")
            });
    }

    return (
        <div className="login-container">
            <div style={{ marginTop: "180px", marginBottom: "118px" }}>
                <img src="Logo.svg" alt="logo" style={{ width: '100%' }}></img>
            </div>
            <Stack style={{marginBottom: '100px'}} spacing={1} >
                <TextField label="שם משתמש" variant="outlined" onChange={(e) => { setUser(e.target.value) }} /> <br />
                <TextField label="סיסמא" type="password" autoComplete="current-password" onChange={(e) => { setPassword(e.target.value) }} />
            </Stack>
            <p>{userExistsMSG}</p>
            <PrimeButton style={{marginTop:'100px'}} onClick={btnlogin} btntxt="כניסה" />
            <p className='newregister'>
                <button onClick={() => navigate('/sign-up')} variant="contained">הירשם</button>
                ?אין לך משתמש
            </p>
        </div>
    )
}
export default LogIn;