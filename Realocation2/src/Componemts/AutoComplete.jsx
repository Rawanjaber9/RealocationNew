import React, { useEffect, useState } from "react";
import TextField from "@mui/material/TextField";
import Autocomplete from "@mui/material/Autocomplete";
import { isLocalhost, Translate } from "../Utils";
import { getLocalStorage, setLocalStorage } from "../utils/functions";

const url = isLocalhost
  ? "http://localhost:5231/api/"
  : "proj.ruppin.ac.il/bgroup30/test2";

export default function AutoComplete(props) {
  const { setInputCountry, defultCountry } = props;
  const [country, setCountry] = useState(defultCountry);
  const [countries, setCountries] = useState([{ label: "" }]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const userId = getLocalStorage("currentUser");
  const selected_country = getLocalStorage(userId).selected_country;

  const setSelectedCountry = (e, newValue) => {
    console.log(e, newValue);
    if (newValue) {
      const user = getLocalStorage(userId);
      user.selected_country = newValue;
      setLocalStorage(userId, user);
      setInputCountry(newValue.label);
      setCountry(newValue)
    }
  };

  useEffect(() => {
    fetch("https://restcountries.com/v3.1/all?fields=name,flags")
      .then((response) => {
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        return response.json();
      })
      .then((data) => {
        const sortedData = data.sort((a, b) =>
          a.name.common.localeCompare(b.name.common)
        );

        const parsedData = sortedData.map((country) => ({
          label: country.name.common,
        }));

        setCountries(parsedData);
        setLoading(false);
      })
      .catch((error) => {
        setError(error.message);
        setLoading(false);
      });
    if (selected_country) {
      setCountry(selected_country);
      setInputCountry(selected_country)
    }
  }, []);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <Autocomplete
      disablePortal
      id="combo-box-demo"
      value={country}
      options={countries}
      onChange={setSelectedCountry}
      renderInput={(params) => <TextField {...params} label="Country" />}
      getOptionLabel={(option) => option.label} // Specify how to display the options
    />
  );
}
