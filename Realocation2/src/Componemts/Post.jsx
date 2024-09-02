import { Avatar, Button, Dialog, DialogActions, Grid, List, ListItem, ListItemAvatar, ListItemText, TextField } from "@mui/material";
import React, { useState, useContext } from "react";
import { UserContext } from "./UserHook";
import { baseURL } from '../Utils';
import { getLocalStorage } from "../utils/functions";

export default function Post({ posts }) {
  const { userDetails } = useContext(UserContext);
  const userId = getLocalStorage("currentUser")
  console.log(userDetails);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const [showAllComments, setShowAllComments] = useState({});
  const [open, setOpen] = useState(false);
  const [commentTo, setCommentTo] = useState([]);
  const url = baseURL();

  const sendComment = (postId) => {
    console.log(postId)
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const raw = JSON.stringify({
      "Content": newComment
    });

    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };

    fetch(`${url}UserPost/add-comment/${postId}/${userId}`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        console.log(result);
        const newId = comments[postId]?.length + 1 || 1;
        const updateComments = [
          ...(comments[postId] || []),
          { id: newId, user: userDetails.userName, content: newComment },
        ];
        setComments(prev => ({
          ...prev,
          [postId]: updateComments,
        }));
        setNewComment("");
      })
      .catch((error) => console.error(error));
  };

  const toggleShowAllComments = (postId) => {
    setShowAllComments(prev => ({
      ...prev,
      [postId]: !prev[postId], // Toggle the visibility of comments for the specific post
    }));
  };

  const openCommentTo = (id) => {
    const comment_to = comments.filter((c) => c.comment_to == id);
    setCommentTo(comment_to);
  };

  const sortedPosts = [...posts].sort((a, b) => b.postId - a.postId);

  return (
    <>
      {sortedPosts.map(post => (
        <div key={post.postId}
          style={{ padding: "24px 16px", backgroundColor: "white", margin: "16px 0", borderRadius: "16px" }}>
          <div style={{ display: "flex", alignItems: "center", marginBottom: "8px" }}>
            <Avatar
              src={post.userProfilePicture ? `data:image/jpeg;base64,${post.userProfilePicture}` : '/static/images/avatar/1.jpg'}
              style={{ marginLeft: "16px" }} />
            <ListItemText style={{ marginRight: "8px", textAlign: "right", margin: "0" }}
              primary={post.username}
              secondary={post.content} />
          </div>
          <Button onClick={() => toggleShowAllComments(post.postId)} style={{ display: "flex", justifyContent: "flex-start" }}>
            {showAllComments[post.postId] ? "לכל התגובות" : "הסתר תגובות"}
          </Button>
          <List style={{ padding: "0", marginTop:"8px", marginBottom: "8px" }}>
            {(comments[post.postId] || [])
              .filter((c) => c.comment_to == null)
              .slice(0, showAllComments[post.postId] ? undefined : 3)
              .map((comment) => (
                <ListItem
                  onClick={() => {
                    openCommentTo(comment.id);
                  }}
                  key={comment.id}
                  style={{ padding: "0" }}>
                  <ListItemAvatar>
                    <Avatar />
                  </ListItemAvatar>
                  <ListItemText
                    style={{ textAlign: "right" }}
                    primary={comment.user}
                    secondary={comment.content} />
                </ListItem>
              ))}
          </List>
          <Grid container spacing={1} style={{ margintop: "8xp" }}>
            <Grid item xs={9}>
              <TextField
                placeholder="הוסף תגובה"
                value={newComment}
                onChange={(e) => setNewComment(e.target.value)} />
            </Grid>
            <Grid item xs={3}>
              <Button style={{ padding: "8px 16px", backgroundColor: "#0C8CE9", color: "white", borderRadius: "40px", fontSize: "16px", fontWeight: "bold", textAlign: "center" }}
                onClick={() => sendComment(post.postId)}>
                שלח
              </Button>
            </Grid>
          </Grid>
          {commentTo.length > 0 && (
            <Dialog
              open={open}
              onClose={setOpen(false)}
              aria-labelledby="alert-dialog-title"
              aria-describedby="alert-dialog-description">
              {commentTo.map((comment) => (
                <ListItem
                  onClick={() => {
                    openCommentTo(comment.id);
                  }}
                  key={comment.id}
                  style={{ padding: "0" }}>
                  <ListItemAvatar>
                    <Avatar
                      src={post.comments.commentUserProfilePicture ? `data:image/jpeg;base64,${comment.commentUserProfilePicture}` : '/static/images/avatar/1.jpg'}
                    />
                  </ListItemAvatar>
                  <ListItemText
                    style={{ textAlign: "right" }}
                    primary={comment.user}
                    secondary={comment.content} />
                </ListItem>
              ))}
              <DialogActions>
                <Button
                  onClick={() => {
                    setOpen(false);
                  }}>
                  close
                </Button>
              </DialogActions>
            </Dialog>
          )}
        </div>))}
      {" "}

    </>
  );
}
