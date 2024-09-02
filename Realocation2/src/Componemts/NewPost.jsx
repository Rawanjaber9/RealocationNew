import React, { useContext, useState } from 'react'
import { Grid, IconButton, TextField } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PrimeButton from './PrimeButton';
import Navbar from './Navbar';
import { baseURL } from '../Utils';
import { UserContext } from './UserHook';
import { getLocalStorage } from '../utils/functions';
import { useNavigate } from 'react-router-dom';

export default function NewPost() {
  const { userDetails } = useContext(UserContext);
  const userId = getLocalStorage("currentUser");
  const [postContent, setPostContent] = useState("");
  const url = baseURL();
  const navigate = useNavigate();

  console.log(userId);

  const handlePost = () => {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const raw = JSON.stringify({
      "content": postContent
    });

    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };

    fetch(`${url}UserPost/add-post/${userId}`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        console.log(result);
        navigate('/post');
      })
      .catch((error) => console.error(error));
  }

  return (
    <div style={{ padding: '16px', height: '88vh' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', width: '100%' }}>
        <IconButton onClick={() => navigate(-1)} style={{ transform: 'scaleX(-1)', position: 'absolute', left: '320px' }}>
          <ArrowBackIcon />
        </IconButton>
        <h4 style={{ textAlign: 'center' }}>שיתוף פוסט</h4>
      </div>
      <TextField
        fullWidth
        multiline
        rows={8}
        placeholder="מה היית רוצה לשתף?"
        variant="outlined"
        value={postContent}
        onChange={(e) => setPostContent(e.target.value)}
        style={{ marginBottom: '24px', direction: 'rtl' }}
      />
      <PrimeButton onClick={handlePost} btntxt="פרסם" />
      <Navbar />
    </div>
  )
}
