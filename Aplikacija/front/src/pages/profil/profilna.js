import React, { useState, useEffect } from "react";
import myImage2 from "./slike/avatar1.jpg";
import "./stilovi_profil.css";
import useCurrentUser from "../../useCurrentUser";
const Profilna = (props) => {
  const [data, setData] = useState();

console.log(props.urlSlike);

const slika=props.urlSlike;
  if (props.urlSlike === undefined || props.urlSlike===null) {
    return <img src={myImage2} alt="My Image" className="myImage" />;
  } else {
    return <img src={slika} alt="My Image" className="myImage" />;
  }
};

export default Profilna;
