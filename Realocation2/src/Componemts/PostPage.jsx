import React, { useEffect, useState } from 'react'
import Navbar from './Navbar'
import { Fab, TextField } from '@mui/material'
import Post from './Post';
import { useNavigate } from 'react-router-dom';
import { getLocalStorage } from '../utils/functions';
import { baseURL } from '../Utils';
import './Realocation.css';


export default function PostPage() {
    const [activeTab, setActiveTab] = useState('general');
    const navigate = useNavigate();
    const [destCountry, setDestCountry] = useState("");
    const [postId, setPostId] = useState(null);
    const userId = getLocalStorage("currentUser");
    const user = getLocalStorage(userId);
    const [posts, setPosts] = useState([]);
    const url = baseURL();

    useEffect(() => {
        const storedCountry = getLocalStorage('selected_country');
        try {
            if (storedCountry) {
                const parsedCountry = typeof storedCountry === 'string' && storedCountry.startsWith('{')
                    ? JSON.parse(storedCountry)
                    : storedCountry;
                setDestCountry(parsedCountry.label ? parsedCountry.label : parsedCountry);
            }
        } catch (error) {
            console.error("Error parsing stored country:", error);
        }
    }, []);

    useEffect(() => {
        loadPosts();
    }, [])

    const loadPosts = () => {

        const requestOptions = {
            method: "GET",
            redirect: "follow"
        };

        fetch(`${url}Userpost/posts-by-destination/${user.selected_country.label}`, requestOptions)
            .then((response) => response.json())
            .then((result) => {
                console.log(result)
                setPosts(result);
            })
            .catch((error) => console.error(error));
    }

    const SelectedMyPosts = (tab) => {
        console.log("selected", tab)
        const requestOptions = {
            method: "GET",
            redirect: "follow"
        };

        fetch(`${url}Userpost/user-posts/${userId}`, requestOptions)
            .then((response) => response.json())
            .then((result) => {
                console.log(result);
                if(result.length > 0){
                const sortedMyPosts = result.sort((a, b) => b.postId - a.postId);
                setPosts(sortedMyPosts);
            }
            else{
                setPosts([]);
            }
            })
            .catch((error) => console.error(error));
    }

    const tabStyle = {
        padding: '10px 20px',
        borderBottom: '2px solid transparent',
        marginBottom: '-2px',
        width: '33%',
        flex: 1,
        fontWeight: 'normal'
    };

    const activeTabStyle = {
        ...tabStyle,
        borderBottom: '2px solid #0C8CE9',
        fontWeight: 'bold'
    };

    const goToNewPost = () => {
        navigate('/new-post');
    };

    return (
        <div style={{ height: '100vh' }} >
            <div style={{ paddingTop: '24px', textAlign: 'center', color: 'white', padding: '0px', margin: '0px' }}>
                <img src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/blueLogo.png" alt="logo" />
            </div>
            <h4 style={{ margin: '8px', textAlign: 'center' }}>{user.selected_country.label}</h4>
            <div dir='rtl'>
                <div style={{ display: 'flex', justifyContent: 'center', direction: 'rtl' }}>
                    <h3 className='MyPostTab'
                        style={activeTab === 'general' ? activeTabStyle : tabStyle}
                        onClick={() => {
                            setActiveTab('general');
                            loadPosts();
                        }}>כללי</h3>
                    <h3
                        style={activeTab === 'myPosts' ? activeTabStyle : tabStyle}
                        onClick={() => {
                            setActiveTab('myPosts');
                            SelectedMyPosts('myPosts');
                        }}>הפוסטים שלי</h3>
                </div>
                <div style={{ margin: '20px' }}>
                    <TextField
                        fullWidth
                        placeholder="מה היית רוצה לשתף?"
                        onClick={goToNewPost}
                        InputProps={{
                            readOnly: true,
                        }} />
                </div>
                <Post posts={posts} />
            </div>
            <Navbar />
        </div>
    )
}
