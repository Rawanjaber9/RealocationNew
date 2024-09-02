import React from 'react'
import { HashRouter as Router, Routes, Route } from 'react-router-dom';
import LogIn from './LogIn'
import OpeningQuestions from './OpeningQuestions'
import Categories from './Categories';
import TaskBoard from './TaskBoard';
import SignUp from './SignUp';
import EditTask from './EditTask';
import HomePage from './HomePage';
import Terms from './Terms';
import RestorePassword from './RestorePassword';
import PostPage from './PostPage';
import NewPost from './NewPost';
import User from './User';

export default function RealocationApp() {

  return (
    <Router>
      <Routes>
        <Route path="/" element={<LogIn />} />
        <Route path="/restore-password" element={<RestorePassword />} />
        <Route path="/sign-up" element={<SignUp />} />
        <Route path="/terms" element={<Terms />} />
        <Route path="/opening-questions" element={<OpeningQuestions />} />
        <Route path="/categories" element={<Categories />} />
        <Route path="/tasks-board" element={<TaskBoard />} />
        <Route path="/edit-task/:taskId" element={<EditTask />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/post" element={<PostPage />} />
        <Route path="/new-post" element={<NewPost />} />
        <Route path="/user" element={<User />} />
      </Routes>
    </Router>
  )
}


