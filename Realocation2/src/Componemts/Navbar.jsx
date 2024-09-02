import { Stack } from '@mui/material'
import { useLocation, useNavigate } from 'react-router-dom';

export default function () {
    const navigate = useNavigate();
    const location = useLocation();
    const isActive = (path) => location.pathname === path;

    const underlineStyle = {
        borderBottom: '3px solid #0C8CE9',
        paddingBottom: '10px',
    };

    return (
        <div style={{
            position: 'fixed',
            display: 'flex',
            justifyContent: 'space-evenly',
            alignItems: 'center',
            height: '73px',
            direction: 'rtl',
            backgroundColor: '#f5f7fa',
            borderTop: '1px solid white',
            bottom: '0',
            left: '0',
            width: '100%',
            zIndex: '1000',
            maxWidth: '393px'
        }}>
            <Stack direction="row" spacing={1} >
                <button style={isActive('/terms') ? { ...underlineStyle } : { backgroundColor: '#f5f7fa' }}
                    onClick={() => navigate('/terms')}>
                    <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/R.png" alt="R" />
                </button>
                <button style={isActive('/tasks-board') ? { ...underlineStyle } : { backgroundColor: '#f5f7fa' }} onClick={() => navigate('/tasks-board')}>
                    <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/Tasks.png" alt="Tasks" />
                </button>
                <button style={isActive('/home') ? { ...underlineStyle } : { backgroundColor: '#f5f7fa' }} onClick={() => navigate('/home')}>
                    <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/HomeIcon.png" alt="Home" />
                </button>
                <button style={isActive('/post') ? { ...underlineStyle } : { backgroundColor: '#f5f7fa' }} onClick={() => navigate('/post')}>
                    <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/PlusIcon.png" alt="Plus" />
                </button>
                <button style={isActive('/user') ? { ...underlineStyle } : { backgroundColor: '#f5f7fa' }} onClick={() => navigate('/user')}>
                    <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/UserIcon.png" alt="User" />
                </button>
            </Stack>
        </div>
    )
}
