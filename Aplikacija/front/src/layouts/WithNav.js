import { NavLink, Outlet, useNavigate } from "react-router-dom"
import useCurrentUser from "../useCurrentUser";
import{useState} from 'react'
import Menu from "./Menu.js"
import SearchBar from "./SearchBar";

let U=[<NavLink to="Profile" key="0">Profil</NavLink>,<NavLink to="Leaderboard"key="1">Rang lista</NavLink>];
let QM=[U,<NavLink to="QuizMaker" key="2">Napravi Kviz</NavLink>]
let A=[U,<NavLink to="Admin" key="3">Admin</NavLink>];

const parseJwt = (token) => {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      throw new Error('Invalid token format');
    }
    const decoded = JSON.parse(atob(parts[1]));
    if (typeof decoded.exp === 'undefined') {
      throw new Error('Token has no expiration');
    }
    return decoded;
  } catch (error) {
    console.error('Failed to parse JWT', error);
    return null;
  }
};

const handleLogOut=() =>{
  localStorage.removeItem("token");
  localStorage.removeItem("Guest");
}
const handleLogIn=()=>{
  localStorage.removeItem("Guest");
}

let lo=<NavLink to="Login" className="LogOut" onClick={handleLogOut}>Odjavi se</NavLink>;
let cu=<NavLink to="ContactUs" className="cu" key="ContactUs">Kontaktirajte nas</NavLink>;

export default function WithNav () {   

  window.addEventListener("scroll", function() {
    const navbar = document.querySelector(".navbar");
    if(navbar)
      navbar.classList.toggle("transparent", window.scrollY !== 0);
  });

    const navigate=useNavigate();
    const [showSearchBar, setShowSearchBar] = useState(false);

    const toggleSearchBar = () => {
      setShowSearchBar(!showSearchBar);
    };

    const [isMenuOpen, setIsMenuOpen] = useState(false);

    const {data:podaci,error,isLoading} = useCurrentUser();

    const user = localStorage.getItem('token');
    if (!user) {
      localStorage.removeItem('token');
      console.log('No JWT found in localStorage')}
    else{
      console.log(user);
      const decodedJwt = parseJwt(user);
      if (decodedJwt && decodedJwt.exp * 1000 < Date.now()) {
        console.log('JWT expired, removing from localStorage');
        localStorage.removeItem('token');
      }
    } 

    if(!localStorage.getItem("token") || localStorage.getItem("token")==="undefined"){
      localStorage.setItem("Guest",true);
      localStorage.removeItem("token");
    }
    const handleClickUser = (username)=>{
      navigate(`/User/${username}`);
    } 
    const refresh=()=>{
      if(window.location.href==="http://localhost:3000/")
        window.location.reload();
    }
    if(localStorage.getItem("Guest")==="true"){
      return (
        <div className="root-layout">        
          <header className="fixed-top navbar">
                <NavLink to="/" className="HomeButton" onClick={()=>{refresh()}}><div className="logoBtn"></div><label>Quiz M²S</label></NavLink>
                <div className="links"> 
                </div>
                <NavLink to="ContactUs" className="contactUs" key="ContactUs">Kontaktirajte nas</NavLink>
                <NavLink to="Login" className="LogIn" onClick={handleLogIn}>Prijavi se</NavLink>
                <button className="menuButton" onClick={() => setIsMenuOpen(!isMenuOpen)}><Menu></Menu></button>

                <ul className={`menu ${isMenuOpen ? 'menu--open' : ''}`}>
                <NavLink to="ContactUs" className="contactUs" key="ContactUs">Kontaktirajte nas</NavLink>
                <NavLink to="Login" className="LogIn" onClick={handleLogIn}>Prijavi se</NavLink>
                </ul>
          </header>        
          <div className="page">
              <Outlet/>
          </div>
        </div>
      );
    }
  else{
  return (
  <div className="root-layout">     
          {error && <div>{ error }</div>}
          {isLoading && <div>Ucitavanje...</div>}
          {podaci &&
          <header className="fixed-top navbar">
          <NavLink to="/" className="HomeButton" onClick={()=>{refresh()}}><div className="logoBtn"></div><label>Quiz M²S</label></NavLink>
              <div className="links"> 
                  {
                    podaci.role === "QuizMaker" ? QM:
                    podaci.role === "Admin" ? A : 
                    podaci.role==="User" ? U : ""
                  }
                  <a onClick={toggleSearchBar}>Pretraži</a>
                  {showSearchBar && <SearchBar onClick={(username)=>{handleClickUser(username)}}/>}
              </div>
              {cu}
              {lo}
              <button className="menuButton" onClick={() => setIsMenuOpen(!isMenuOpen)}><div><Menu></Menu></div></button>

              <div className={`menu ${isMenuOpen ? 'menu--open' : ''}`}>
                <SearchBar onClick={(username)=>{handleClickUser(username)}}/>
                {
                  podaci.role === "QuizMaker" ? QM:
                  podaci.role === "Admin" ? A : 
                  podaci.role==="User" ? U: ""
                }
                {<NavLink to="ContactUs" className="contactUs" key="ContactUs">Kontaktirajte nas</NavLink>}
                {lo}
              </div>
          </header>
          }
          <div className="page">
              <Outlet/>
          </div>
  </div>
)
}
}