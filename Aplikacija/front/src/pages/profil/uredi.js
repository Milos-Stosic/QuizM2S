import React, { useState, useEffect } from "react";
import Profil from "./profil";
import useCurrentUser from "../../useCurrentUser";
import Profilna from "./profilna";
import Login from "../LoginPage/Login/Login"
import { useNavigate } from "react-router-dom";
const Uredi = () => {
  const [data, setData] = useState();
  const [showPopup, setShowPopup] = useState(false);

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
  }, []);

  const [errorMessagePassword, setErrorMessagePassword] = useState("");
  const [errorMessageUserName, setErrorMessageUserName] = useState("");
  const [errorPhoto, setErrorPhoto] = useState("");
  //  console.log(data);

  const [isChecked, setIsChecked] = useState(false);
  const handleClick = () => {
    setIsChecked(!isChecked);
  };

  const [zeliKK, setKK] = useState(false);
  const handleOptionChange = () => {
    setKK(handleClick);
  };

  console.log("checkf", { isChecked });
  console.log("zeli", { zeliKK });

  const handleOtkazi = () => {
    window.location.reload();
  };

  const [inputValueBio, setInputValueBio] = useState(data?.bio);

  const bioCh = (event) => {
    setInputValueBio(event.target.value);
  };

  const [inputValueUrl, setInputValueUrl] = useState(data?.profilePicture);

  const profilnaCh = (event) => {
    setErrorPhoto("");
    setInputValueUrl(event.target.value);
  };

  const [inputValueName, setInputValueName] = useState(data?.name);

  const userNCh = (event) => {
    setErrorMessageUserName("");
    setInputValueName(event.target.value);
  };

  const [old_pass, setOldPass] = useState("");
  const passCh = (event) => {
    setErrorMessagePassword("");
    setOldPass(event.target.value);
  };

  const [new_pass, setNewPass] = useState("");
  const newP = (event) => {
    setErrorMessagePassword("");
    setNewPass(event.target.value);
  };

  const [conf_pass, setConfPass] = useState("");
  const confP = (event) => {
    setErrorMessagePassword("");
    setConfPass(event.target.value);
  };

  const [checkErrors, setCheckErrors] = useState();
  const navigate = useNavigate();
  const [profilePicture, setProfilePicture] = useState();

  const [obrisiP, setObrisiP] = useState(false);
  const Obrisi = async () => {
    fetch("http://localhost:5013/User/DeleteAccount", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    }).then((res) => {
      if (!res.ok) console.log("greska kod brisanja");
      else {
        localStorage.clear();
        navigate("/Login");
      }
    });
  };
  console.log(obrisiP);

  const handleSacuvaj = () => {
    window.location.reload();
  };

  const saveUsername = async () => {
    if (
      inputValueName !== null &&
      inputValueName !== undefined
    ) {
      await fetch(
        `http://localhost:5013/User/ChangeNameOfUser/${inputValueName}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      )
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
              setErrorMessageUserName(text);
            });
          } else {
            console.log("uspeh");
            setErrorMessageUserName("");
          }
        })
        .catch((err) => {
          console.log(err);
        });
    }
  };

  const savePassword = async () => {
    if (
      old_pass !== null &&
      new_pass !== null &&
      conf_pass !== null &&
      old_pass !== undefined &&
      new_pass !== undefined &&
      conf_pass !== undefined &&
      old_pass !== "" &&
      new_pass !== "" &&
      conf_pass !== ""
    ) {
      await fetch(
        `http://localhost:5013/Login/ChangePassword/${old_pass}/${new_pass}/${conf_pass}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      )
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
              setErrorMessagePassword(text);
            });
          } else {
            setErrorMessagePassword("");
          }
        })
        .catch((err) => {
          console.log(err);
        });
    }
  };

  const savePhoto = async () => {
    if (inputValueUrl !== undefined && inputValueUrl !== null) {
      await fetch(`http://localhost:5013/User/ChangeProfilePhoto`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(`${inputValueUrl}`),
      })
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
              setErrorPhoto(text);
            });
          } else {
            setErrorPhoto("");
            console.log("uspeh");
          }
        })
        .catch((err) => {
          console.log(err);
        });
    }
  };

  const saveBio = async () => {
    if (inputValueBio !== undefined && inputValueBio !== null)
      await fetch(
        `http://localhost:5013/User/ChangeBioOfUser/${inputValueBio}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      )
        .then((res) => {
          if (res.ok) console.log("Uspeh");
        })
        .catch((err) => {
          console.log(err);
        });
  };
  return (
    <div className="profilBasSveU">
      <div className="profilSve">
        <div className="profilU">
          <div className="profilna">
            <div>
              <Profilna urlSlike={profilePicture} />
            </div>
            <div>
              <button
                className="dugme3"
                onClick={() => setShowPopup(true)}
                title="Klikom na ovo dugme obrisaćete profil nakon klika na dugme sačuvaj!!!
                     Otkažite promene da ne bi obrisali profil ako ste greškom kliknuli!"
              >
                {"Obriši profil"}
              </button>
              {showPopup && (
                <div className="popup">
                  <p>Da li ste sigurni da zelite da obrisete profil?</p>
                  <button onClick={() => Obrisi()}>Obrisi</button>
                </div>
              )}
            </div>
            {/* <div>
                   <button className='dugme3'onClick={() => promenaLozinke()}>{"Promeni lozinku"}</button>
                   </div> */}
          </div>

          <div className="korisnik">
            <div className="nickname">
              <textarea
                className="txtAN"
                cols={15}
                placeholder="Unesite novo ime"
                onChange={userNCh}
                defaultValue={data?.name}
              />
              <button className="sacuvaj-btn" onClick={saveUsername}>
                Sacuvaj
              </button>
              <label className="error">{errorMessageUserName}</label>
            </div>
          </div>
          <div className="SaOt">
            <div className="sacuvaj">
              <button
                className="dugme1"
                onClick={() => handleSacuvaj()}
                title="Klikom na ovo dugme sačuvaćete sve promene!"
              >
                {"Nazad"}
              </button>
            </div>
          </div>
        </div>
        <div className="linija"></div>

        <div className="donji">
          <label className="bioLab">Biografija:</label>

          <textarea
            className="bioEdit"
            rows={10}
            cols={100}
            placeholder="Napišite nešto o sebi"
            onChange={bioCh}
            value={inputValueBio}
          />
          <button className="sacuvaj-btn" onClick={saveBio}>
            Sacuvaj
          </button>
          <label className="profLab">Url profilne fotografije:</label>

          <textarea
            className="ProfilnaCh"
            rows={1}
            cols={100}
            placeholder="Unesite URL fotografije"
            onChange={profilnaCh}
            value={inputValueUrl}
          />
          <button className="sacuvaj-btn" onClick={savePhoto}>
            Sacuvaj
          </button>
          <label className="error">{errorPhoto}</label>

          <label className="lozinkaLab">Izmenite lozinku:</label>

          <input
            className="lozinkaCh"
            type="password"
            placeholder="Unesite staru lozinku"
            onChange={passCh}
          />

          <input
            className="lozinkaCh"
            type="password"
            placeholder="Unesite novu lozinku"
            onChange={newP}
          />

          <input
            className="lozinkaCh"
            type="password"
            placeholder="Potvrdite novu lozinku"
            onChange={confP}
          />
          <button className="sacuvaj-btn" onClick={savePassword}>
            Sacuvaj
          </button>
          <label className="error">{errorMessagePassword}</label>
        </div>
      </div>
    </div>
  );
};

export default Uredi;
