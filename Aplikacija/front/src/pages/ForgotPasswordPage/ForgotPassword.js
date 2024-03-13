import {
  createBrowserRouter,
  Route,
  createRoutesFromElements,
  RouterProvider,
  Navigate,
  useNavigate,
} from "react-router-dom";
import { useState } from "react";
import "../ForgotPasswordPage/ForgotPassword.css";

const ForgotPassword = () => {
  const [password, setPassword] = useState();
  const [repeatPassword, setRepeatPassword] = useState(null);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const [email, setEmail] = useState();
  const navigate = useNavigate();
  const [showBack, setShowBack]=useState(false);
  const handleChangeSubmit = async (e) => {
    e.preventDefault();
    try {
      await fetch(
        `http://localhost:5013/Login/ChangePasswordEmail/${email}/${password}/${repeatPassword}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        }
      )
        .then((res) => {
          if (!res.ok) {
            res.text().then((text) => {
              setErrorMessage(text);
              throw text;
            });
          } else {
            setErrorMessage("Uspesno ste promenili lozinku sada se mozete ulogovati");
            setShowBack(true);
          }
        })
        .catch((error) => {
          console.log(error);
        });
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <div className="forgot-div">
      <form className="form-forgot" onSubmit={handleChangeSubmit}>
        <h1 className="title">Promenite lozinku</h1>
        <input
          className="login-input"
          type="email"
          placeholder="Email"
          onChange={(e) => setEmail(e.target.value)}
        ></input>
        <input
          className="login-input"
          type="password"
          placeholder="Lozinka"
          onChange={(e) => setPassword(e.target.value)}
        ></input>
        <input
          className="login-input"
          type="password"
          placeholder="Potvrda lozinke"
          onChange={(e) => setRepeatPassword(e.target.value)}
        ></input>
        {!loading && <button className="login-button">Potvrdi</button>}
        {loading && (
          <button className="login-button" disabled>
            Ucitavanje...
          </button>
        )}
        <div className="error">{errorMessage}</div>
      </form>
      {showBack && <button className="backBtn" onClick={()=>{navigate("/Login")}}>Vrati se na početnu stranicu</button>}
    </div>
  );
};

export default ForgotPassword;
