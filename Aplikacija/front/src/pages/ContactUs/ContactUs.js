import React, { useRef, useEffect } from "react";
import "../ContactUs/ContactUs.css";
import { useState } from "react";
import slika from "../ContactUs/aqua.png";
export const ContactUs = () => {
  const form = useRef();
  const [message, setMessage] = useState("");
  const [errorSending, setErrorSending] = useState("");
  const [roleChange, setRoleChange] = useState("");
  const [loading, setLoading] = useState(false);
  const [email, setEmail] = useState();

  const sendEmail = (e) => {
    console.log(roleChange);
    console.log(message);
    e.preventDefault();

    setLoading(true);

    if (message !== undefined && message !== "" && message!==" ") {
      fetch(`http://localhost:5013/User/SendEmail/${email}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(`${message}`),
      })
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
              setErrorSending(text);
              setLoading(false);
            });
          }
          res.text().then((text) => {
            console.log(text);
            if (roleChange === "Kreator kviza") {
              fetch(`http://localhost:5013/User/WantQuizMaker`, {
                method: "PUT",
                headers: {
                  "Content-Type": "application/json",
                  Authorization: `Bearer ${localStorage.getItem("token")}`,
                },
              })
                .then((res) => {
                  if (!res.ok) {
                    if (res.status === 401) {
                      setErrorSending(
                        "Morate biti prijavljeni da bi posalli zahtev za promenu uloge."
                      );
                    } else {
                      res.text().then((text) => {
                        console.log(text);
                        setErrorSending(
                          "Zao nam je, doslo do greske prilikom slanja zahteva. Pokusajte ponovo kasnije."
                        );
                      });
                    }
                  } else {
                    res.text().then((text) => {
                      console.log(text);
                    });
                  }
                })
                .catch((err) => {
                  console.log(err);
                });
            }

            if (roleChange === "Admin") {
              fetch(`http://localhost:5013/User/WantAdmin`, {
                method: "PUT",
                headers: {
                  "Content-Type": "application/json",
                  Authorization: `Bearer ${localStorage.getItem("token")}`,
                },
              })
                .then((res) => {
                  if (!res.ok) {
                    res.text().then((text) => {
                      console.log(text);
                      setErrorSending(
                        "Zao nam je, doslo do greske prilikom slanja zahteva. Pokusajte ponovo kasnije."
                      );
                    });
                  } else {
                    res.text().then((text) => {
                      console.log(text);
                    });
                  }
                })
                .catch((err) => {
                  console.log(err);
                });
            }
            setLoading(false);
            setErrorSending("Uspesno poslat email!");
          });
        })
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              console.log(text);
              setLoading(false);
              setErrorSending("Greska prilikom slanja mejla");
            });
          }
        })
        .then(() => {
          window.location.reload();
        })
        .catch((err) => {
          console.log(err);
          setErrorSending("Greska pri slanju. Pokusajte sa outlook email-om.");
        });
    }
    else{
      setErrorSending("Ne mozete poslati praznu poruku");
      setLoading(false);

    }
  };

  const handleRoleAdmin = () => {
    setRoleChange("Admin");
  };
  const handleRoleQM = () => {
    setRoleChange("Kreator kviza");
  };

  const handleRoleCancel = () => {
    setRoleChange("");
  };

  return (
    <>
      <div className="contactUs-div">
        <form className="form-contact" onSubmit={sendEmail}>
          {localStorage.getItem("Guest") === "true" ? (
            <>
              <label className="label-contact">Email</label>
              <input
                className="input-contact"
                onChange={(e) => setEmail(e.target.value)}
                type="email"
              />
            </>
          ) : (
            <div className="role-div">
              <label className="label-contact">Odaberite ulogu</label>
              <label
                className={
                  roleChange === "Kreator kviza"
                    ? "role-button-picked"
                    : "role-button"
                }
                onClick={handleRoleQM}
              >
                Kreator kviza
              </label>
              <label
                className={
                  roleChange === "Admin" ? "role-button-picked" : "role-button"
                }
                onClick={handleRoleAdmin}
              >
                Admin
              </label>
              <label
                className="role-button-cancel"
                type=""
                onClick={handleRoleCancel}
              >
                ❌
              </label>
            </div>
          )}
          <label className="label-contact">Poruka</label>
          <textarea
            className="textarea-contact"
            onChange={(e) => setMessage(e.target.value)}
          />
          {!loading && (
            <input className="contact-btn" type="submit" value="Posalji" />
          )}
          {loading && (
            <input
              className="contact-btn"
              type="submit"
              disabled
              value="Slanje..."
            />
          )}
          <label className="error-contact">{errorSending}</label>
        </form>
        <img src={slika} className="slika" />
      </div>
    </>
  );
};
