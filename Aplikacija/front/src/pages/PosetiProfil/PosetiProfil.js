import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Profilna from "../profil/profilna";


const PosetiProfil = ()=>{
    const { username }= useParams();
    const [data, setData] = useState();
    const [profilePicture, setProfilePicture] = useState();
    const [score,setScore]=useState(0);
  

    useEffect(() => {
        fetch(`http://localhost:5013/User/GetDataForUser/${username}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        })
          .then((res) => {
            if (!res.ok) {
              res.text().then((text) => {
              });
            } else {
              res.json().then((text) => {
                setData(text);
                setProfilePicture(text.profilePicutre);
              });
            }
          })
          .catch((err) => {
            console.log(err);
          });
    
          fetch(`http://localhost:5013/Score/GetSumOfScoresOfUser/${username}`,{
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          })
            .then((res) => {
              if (!res.ok) {
                res.text().then((text) => {
                });
              } else {
                res.json().then((text) => {
                  setScore(text);
                });
              }
            })
            .catch((err) => {
              console.log(err);
            });
      }, [username]);
    
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
            </div>
            <div className="linija"></div>
  
            {/* <label className="bioLab">Biografija:</label> */}
  
            <div className="donji">
              <textarea
                className="bio"
                rows={10}
                cols={100}
                placeholder="Korisnik nema biografiju."
                readOnly={true}
                value={data===undefined || data===null || data.bio==null || data==="" ? "" : data.bio}
              ></textarea>
            </div>
          </div>
        </div>
      );
}
export default PosetiProfil;