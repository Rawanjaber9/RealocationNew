import "./Realocation.css";
import React, { useEffect, useState, useContext } from "react";
import TextField from "@mui/material/TextField";
import Stack from "@mui/material/Stack";
import Grid from "@mui/material/Grid";
import PrimeButton from "./PrimeButton";
import SecButton from "./SecButton";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { IconButton } from "@mui/material";
import AutoComplete from "./AutoComplete";
import { UserContext } from "./UserHook";
import { baseURL } from "../Utils";
import { getLocalStorage, setLocalStorage } from "../utils/functions";

function OpeningQuestions(props) {
  const { parseUserData } = props;
  const userId = getLocalStorage("currentUser");
  console.log(userId);
  const { setUserDetails } = useContext(UserContext);
  const [selectedOption, setSelectedOption] = useState("");
  const [day, setDay] = useState("");
  const [month, setMonth] = useState("");
  const [year, setYear] = useState("");
  const [move_date, set_move_date] = useState({});
  const user = getLocalStorage(userId);
  const currentYear = new Date().getFullYear();
  const [errors, setErrors] = useState({
    day: false,
    month: false,
    year: false,
  });
  const [inputCountry, setInputCountry] = useState("");
  const url = baseURL();

  useEffect(() => {
    const userId = getLocalStorage("currentUser");
    const moveDate = getLocalStorage(userId).moveDate;
    const have_kids = getLocalStorage(userId).have_kids;
    setSelectedOption(have_kids)
    set_move_date(moveDate)
    if (moveDate) {
      setDay(moveDate.day);
      setMonth(moveDate.month);
      setYear(moveDate.year);
    }
  }, []);

  const isFormValid = () => {
    return (
      inputCountry &&
      day &&
      month &&
      year &&
      selectedOption &&
      !errors.day &&
      !errors.month &&
      !errors.year
    );
  };

  const SaveDetails = () => {
    if (!isFormValid()) return;
    parseUserData(
      {
        UserId: userId,
        DestinationCountry: inputCountry,
        MoveDate: new Date(year, month - 1, day),
        HasChildren: selectedOption === "yes",
        SelectedCategories: [],
      },
      "cetegories"
    );
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    const data = {
      UserId: userId,
      DestinationCountry: inputCountry,
      MoveDate: new Date(year, month - 1, day),
      HasChildren: selectedOption === "yes",
      SelectedCategories: [],
    }
    const raw = JSON.stringify(data);
    setUserDetails(data)
    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
    };

    fetch(`${url}details`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`error:" ${response.status}`);
        }
        return response.json();
      })
      .then((result) => {
      })
      .catch((error) => console.error(error));
  };

  const validateField = (name, value) => {
    let isValid = true;
    switch (name) {
      case "day":
        isValid = value >= 1 && value <= 31;
        break;
      case "month":
        isValid = value >= 1 && value <= 12;
        break;
      case "year":
        isValid = value >= currentYear - 3 && value <= currentYear + 120;
        break;
      default:
        break;
    }
    setErrors((prev) => ({ ...prev, [name]: !isValid }));
    return isValid;
  };

  const handleChange = (name, value) => {
    let temp;
    if (value) {
      temp = move_date ?? {};
      temp[name] = value;
    }
    set_move_date(temp);
    switch (name) {
      case "day":
        setDay(value);
        validateField("day", value);
        break;
      case "month":
        setMonth(value);
        validateField("month", value);
        break;
      case "year":
        setYear(value);
        validateField("year", value);
        break;
      default:
        break;
    }
    user.moveDate = temp;
    setLocalStorage(userId, user);
  };


  const handleButton = (selection) => {
    setSelectedOption(selection);
    user.have_kids = selection;
    setLocalStorage(userId, user);
  };

  return (
    <div className="OQ-container">
      <div className="stepIndicator" dir="rtl">
        <div className="dot"></div>
        <div className="dot active"></div>
        <div className="dot"></div>
        <div className="dot"></div>
      </div>
      <Grid
        container
        spacing={2}
        alignItems="center"
        style={{ padding: "0 16px", marginBottom: "20%" }}>
        <Grid item xs={1}>
          <IconButton
            onClick={() => parseUserData({}, "terms")}
            style={{ transform: "scaleX(-1)", left: "280px" }}>
            <ArrowBackIcon />
          </IconButton>
        </Grid>
        <Grid item xs={11}>
          <h4 style={{ textAlign: "center" }}>שאלות לדיוק התהליך</h4>
        </Grid>
      </Grid>
      <Stack spacing={3} style={{ marginBottom: "50%" }}>
        <p style={{ textAlign: "right" }}>מדינת יעד</p>
        <AutoComplete
          setInputCountry={setInputCountry}
          defultCountry={inputCountry} />
        <div>
          <p style={{ textAlign: "right" }}>מתי המעבר</p>
          <Grid
            container
            spacing={1}
            justifyContent={"center"}
            alignItems="center">
            <Grid item xs={4}>
              <TextField
                fullWidth
                id="year"
                label="שנה"
                type="number"
                variant="outlined"
                name="year"
                value={year}
                onChange={(e) => handleChange("year", e.target.value)}
                error={errors.year} />
            </Grid>
            <Grid item xs={4}>
              <TextField
                fullWidth
                id="month"
                label="חודש"
                type="number"
                variant="outlined"
                name="month"
                value={month}
                onChange={(e) => handleChange("month", e.target.value)}
                error={errors.month} />
            </Grid>
            <Grid item xs={4}>
              <TextField
                fullWidth
                id="day"
                label="יום"
                type="number"
                variant="outlined"
                name="day"
                value={day}
                onChange={(e) => handleChange("day", e.target.value)}
                error={errors.day} />
            </Grid>
          </Grid>
          <br />
        </div>
        <Grid container alignItems="center">
          <Grid item xs={3.5}>
            <SecButton
              btntxt="לא"
              onClick={() => handleButton("no")}
              active={selectedOption === "no"} />
          </Grid>
          <Grid item xs={3.5}>
            <SecButton
              btntxt="כן"
              onClick={() => handleButton("yes")}
              active={selectedOption === "yes"} />
          </Grid>
          <Grid item xs={5}>
            <p>?האם יש לך ילדים</p>
          </Grid>
        </Grid>
      </Stack>
      <PrimeButton
        onClick={SaveDetails}
        btntxt="הבא"
        disabled={!isFormValid()} />
    </div>
  );
}

export default OpeningQuestions;
