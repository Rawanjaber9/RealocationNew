import React, { useContext, useState, useEffect } from "react";
import { Stack, TextField } from "@mui/material";
import "./Realocation.css";
import PrimeButton from "./PrimeButton";
import { useNavigate } from "react-router-dom";
import { baseURL } from "../Utils";
import { UserContext } from "./UserHook";
import { getLocalStorage, setLocalStorage } from "../utils/functions";

function SignUp() {
  const [user, setUser] = useState({
    fullname: "",
    email: "",
    password: "",
    confirmpassword: "",
  });
  const [userExistsMSG, setUserExistsMSG] = useState("");
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();
  const { setUserDetails } = useContext(UserContext);
  const url = baseURL();

  const handleRegister = () => {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json; charset=utf-8");
    myHeaders.append("Accept", "application/json; charset=utf-8");

    const RegistrationData = JSON.stringify({
      email: user.email,
      fullName: user.fullname,
      passwordHash: user.password,
      username: user.email,
    });

    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: RegistrationData,
      redirect: "follow",
    };
    const errors = Object.keys(user)
      .map((key) => validateField(key, user[key]))
      .filter((error) => error);
    if (errors.length === 0) {
      fetch(`${url}register/register`, requestOptions)
        .then((response) => {
          if (!response.ok) throw new Error("field to register");
          return response.json();
        })
        .then((result) => {
          console.log("work");
          console.log(result);
          console.log(result.userId);
          setUserDetails({ userId: result.userId });
          if (!getLocalStorage(result.userId)) {
            setLocalStorage(result.userId, {
              category_active: [],
              have_kids: "no",
              moveDate: { year: "", month: "", day: "" },
              selected_country: { label: "" },
              completeReg: false,
            });
          }
          setLocalStorage("currentUser", result.userId);
          navigate("/terms", {
            state: { userId: result.userId },
            state: { fromReg: true },
          });
        })
        .catch((error) => {
          console.log("not work");
          console.log(error);
          setUserExistsMSG("משתמש קיים");
        });
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setUser((prevState) => ({
      ...prevState,
      [name]: value,
    }));
    validateField(name, value);
  };

  const validateField = (name, value) => {
    let errMsg = "";
    switch (name) {
      case "fullname":
        if (value.split(" ").length < 2) {
          errMsg = "יש לרשום שם מלא";
        }
        break;
      case "email":
        if (
          !value.includes("@") ||
          !value.endsWith(".com") ||
          !/[A-Za-z]/.test(value)) {
          errMsg = "כתובת מייל לא תקינה";
        }
        break;
      case "password":
        if (!/^[A-Za-z0-9]{6,}$/.test(value)) {
          errMsg = "סיסמא צריכה להכיל 6 תווים לפחות";
        }
        break;
      case "confirmpassword":
        if (value !== user.password) {
          errMsg = "סיסמא לא תואמת";
        }
        break;
      default:
        break;
    }
    setErrors((prev) => ({ ...prev, [name]: errMsg }));
    return errMsg;
  };

  useEffect(() => {
    localStorage.clear();
  }, []);
  const handleBlur = (e) => {
    const { name, value } = e.target;
    validateField(name, value);
  };

  return (
    <div className="signup-container">
      <img
        className="logo"
        src="https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/Logo.svg"
        alt="logo"
        style={{ marginTop: "80px" }} />
      <div className="signup-inputs">
        <TextField
          label="שם מלא"
          name="fullname"
          variant="outlined"
          value={user.fullname}
          onChange={handleChange}
          onBlur={handleBlur}
          error={!!errors.fullname}
          helperText={errors.fullname} />
        <TextField
          label="מייל"
          name="email"
          type="email"
          variant="outlined"
          value={user.email}
          onChange={handleChange}
          onBlur={handleBlur}
          error={!!errors.email}
          helperText={errors.email} />
        <TextField
          label="סיסמא"
          name="password"
          type="password"
          variant="outlined"
          value={user.password}
          onChange={handleChange}
          onBlur={handleBlur}
          error={!!errors.password}
          helperText={errors.email} />
        <TextField
          label="אימות סיסמא"
          name="confirmpassword"
          type="password"
          variant="outlined"
          value={user.confirmpassword}
          onChange={handleChange}
          onBlur={handleBlur}
          error={!!errors.confirmpassword}
          helperText={errors.confirmpassword} />
      </div>
      <p>{userExistsMSG}</p>
      <Stack spacing={1}>
        <PrimeButton onClick={handleRegister} btntxt="הירשם" />
        <button onClick={() => navigate("/")} variant="contained">
          לחשבון קיים
        </button>
      </Stack>
    </div>
  );
}
export default SignUp;
