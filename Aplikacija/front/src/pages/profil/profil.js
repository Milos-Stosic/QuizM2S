import React, { useState, useEffect } from "react";
import Uredi from "./uredi";
import "./stilovi_profil.css";
import useFetch from "../../useFetch";
import useCurrentUser from "../../useCurrentUser";
import Profilna from "./profilna";

const Profil = () => {
  const [data, setData] = useState();
  const [profilePicture, setProfilePicture] = useState();
  const [score,setScore]=useState(0);

  const [strana, setStrana] = useState(1);
  const handleClick1 = (strana) => {
    setStrana(2);
  };

  useEffect(() => {
    fetch(`http://localhost:5013/User/GetCurrentUserData`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    })
      .then((res) => {
        if (!res.ok) {
          res.text().then((text) => {
            console.log(text);
          });
        } else {
          res.json().then((text) => {
            console.log(text);
            setData(text);
            setProfilePicture(text.profilePicutre);
          });
        }
      })
      .catch((err) => {
        console.log(err);
      });

      fetch(`http://localhost:5013/Score/GetSumOfScores`,{
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      })
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
            });
          } else {
            res.json().then((text) => {
              console.log(text);
              setScore(text);
            });
          }
        })
        .catch((err) => {
          console.log(err);
        });
  }, []);

  if (strana === 1)
    return (
      <div className="profilBasSve">
        <div className="profilSve">
          <div className="profil">
            <div className="profilna">
              <Profilna urlSlike={profilePicture} />
            </div>

            <div className="korisnik">
              <div className="nickname">
                <label className="labela">{data?.name}</label>
              </div>

              <div className="brojP">
                <label className="labela">
                  Broj rešenih kvizova: {data?.numberOfQuizzesDone}
                </label>
                {/* <label className="labela">Broj resenih kvizova: {korisnik.brojQ}</label> */}
              </div>

              <div className="brojQ">
                <label className="labela">
                  Broj osvojenih poena: {score}
                </label>
                {/* <label className="labela">
                  Broj osvojenih poena: {korisnik.brojP}
                </label> */}
              </div>
            </div>

            <div className="uredi">
              <button className="dugme" onClick={handleClick1}>
                {" "}
                Uredi{" "}
              </button>
            </div>
          </div>
          <div className="linija"></div>

          <label className="bioLab">Biografija:</label> 

          <div className="donji">
            <textarea
              className="bio"
              rows={10}
              cols={100}
              placeholder="Uredi profil da bi dodao biografiju!"
              readOnly={true}
              value={data===undefined || data===null || data.bio==null || data==="" ? "" : data.bio}
            ></textarea>
          </div>
        </div>
      </div>
    );
  else return <Uredi />;
};

export default Profil;
