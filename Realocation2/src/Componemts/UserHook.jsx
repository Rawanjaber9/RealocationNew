import React, { useState, createContext } from "react";
import { getLocalStorage, setLocalStorage } from "../utils/functions";

export const UserContext = createContext(null);

export const UserHook = () => {
  const [userDetails, set_UserDetails] = useState(getLocalStorage('userDetails') ?? null);
  const setUserDetails = (value) => {
    setLocalStorage('userDetails', value)
    set_UserDetails(value)
  };
  return {
    userDetails,
    setUserDetails,
  };
};
