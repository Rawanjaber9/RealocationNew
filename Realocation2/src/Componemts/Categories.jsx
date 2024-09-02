import React, { useState, useEffect } from 'react';
import { Grid, IconButton } from '@mui/material';
import CategoryItem from './CategoryItem';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PrimeButton from './PrimeButton';
import { baseURL } from '../Utils';
import { getLocalStorage, setLocalStorage } from '../utils/functions';
import { Navigate } from 'react-router-dom';


export default function Categories(props) {

  const { parseUserData, userData } = props
  console.log(userData);
  const userId = getLocalStorage("currentUser");
  const user = getLocalStorage(userId);
  const url = baseURL();
  const [active, setActive] = useState([]);
  const initialCategories = [
    { id: 1, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/animals.png", label: "בעלי חיים" },
    { id: 2, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/flight.png", label: "טיסה" },
    { id: 3, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/working.png", label: "עבודה" },
    { id: 4, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/doctor.png", label: "בריאות" },
    { id: 5, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/home.png", label: "מגורים" },
    { id: 6, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/beer.png", label: "פנאי" },
    { id: 7, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/classroom.png", label: "חינוך ילדים" },
    { id: 8, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/truck.png", label: "הובלה" },
    { id: 9, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/student.png", label: "חינוך בוגרים" },
    { id: 10, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/shild.png", label: "ביטוחים" },
    { id: 11, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/car.png", label: "רכב" },
    { id: 12, image: "https://proj.ruppin.ac.il/bgroup30/test2/tar2/dist/friends.png", label: "קהילות" }
  ];
  const [categories, setCategories] = useState(initialCategories);
  useEffect(() => {
    fetchSelectedCategories();
    const kids = user.have_kids === "yes" ? true : false;
    const filteredCategories = kids
      ? initialCategories
      : initialCategories.filter(Category => Category.label !== "חינוך ילדים");
    console.log(filteredCategories)
    setCategories(filteredCategories);
    const category_active = user.category_active;
    if (category_active) {
      setActive(category_active)
    }
  }, [])

  function fetchSelectedCategories() {
    const requestOptions = {
      method: "GET",
      redirect: "follow"
    };

    fetch(`${url}UserCategories/tasks/user/${userData.UserId}/true`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        const selectedIds = result.map(item => item.categoryId);
        const updatedCategories = initialCategories.map(cat => ({
          ...cat,
          active: selectedIds.includes(cat.id)
        }));
        setCategories(updatedCategories);
      })
      .catch((error) => console.error(error));
  }

  const SaveCateegories = () => {

    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const moveDateObj = user.moveDate;
    const fullMoveDate = new Date(moveDateObj.year, moveDateObj.month - 1, moveDateObj.day);
    if (isNaN(fullMoveDate)) {
      console.error("Invalid date: ", fullMoveDate);
      return;
    }

    const saveDB = getLocalStorage('saveDB');
    if (saveDB) {
      parseUserData({ SelectedCategories: active }, "taskBoard");
      return
    }

    console.log("active categories:", active);

    const raw = JSON.stringify({
      "UserId": userId,
      "DestinationCountry": user.selected_country.label,
      "MoveDate": fullMoveDate.toISOString(),
      "HasChildren": user.have_kids === "yes" ? true : false,
      "SelectedCategories": active
    });

    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };

    fetch(`${url}Details/save-details-and-calculate/${userId}`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
        setLocalStorage('saveDB', 1);
        parseUserData({ SelectedCategories: active }, "taskBoard");
      })
      .catch((error) => console.error(error));


    return;
  }

  const toggleActive = (id) => {
    setActive(currentActive => {
      if (currentActive.includes(id)) {
        return currentActive.filter(item => item !== id);
      }
      else {
        return [...currentActive, id];
      }
    });
  };
  useEffect(() => {
    const user = getLocalStorage(userId)
    if (active && active.length > 0) {
      user.category_active = active;
      setLocalStorage(userId, user)
    } else {
      user.category_active = []
      setLocalStorage(userId, user)
    }

  }, [active])
  return (
    <div className='Categories-container'>
      <div className='stepIndicator' dir='rtl' >
        <div className='dot'></div>
        <div className='dot'></div>
        <div className='dot active'></div>
        <div className='dot'></div>
      </div>
      <div style={{ display: 'flex', alignItems: 'center', padding: ' 0 16px' }}>
        <IconButton onClick={() => parseUserData({}, "opningQuestions")} style={{ transform: 'scaleX(-1)', left: '280px' }}>
          <ArrowBackIcon />
        </IconButton>
        <h4 style={{ textAlign: 'center' }}>בחירת נושאי משימות </h4>
      </div>
      <Grid container direction="row-reverse" justifyContent="center" spacing={0.5}>
        {categories.map(Category => (
          <Grid key={Category.id} item xs={4}>
            <CategoryItem
              image={Category.image}
              label={Category.label}
              active={active.includes(Category.id)}
              onClick={() => toggleActive(Category.id)} />
          </Grid >
        ))}
      </Grid>
      <div style={{ padding: '16px', display: 'flex', justifyContent: 'center' }}>
        <PrimeButton onClick={SaveCateegories} btntxt="הבא" disabled={active.length === 0} />
      </div>
    </div>
  );
}
