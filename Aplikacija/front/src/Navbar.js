import { createBrowserRouter, Route, createRoutesFromElements, RouterProvider } from "react-router-dom";
import { useState } from "react";

//pages
import QuizMaker from "./pages/QuizMaker/QuizMaker";

//layouts
import WithNav from "./layouts/WithNav";
import WithoutNav from "./layouts/WithoutNav";
import Login from "./pages/LoginPage/Login/Login";
import PlayQuizStartPage from "./pages/PlayQuizStartPage/PlayQuizStartPage";
import Profil from "./pages/profil/profil"
import Leaderboard from "./pages/leaderboard/leaderboard";
import Admin from "./pages/Admin/Admin"
import PosetiProfil from "./pages/PosetiProfil/PosetiProfil";
import { ContactUs } from "./pages/ContactUs/ContactUs";
import ForgotPassword from "./pages/ForgotPasswordPage/ForgotPassword";

const Navbar = props => {
    const [auth,setAuth]=useState("User");
    
    const router = createBrowserRouter(
        createRoutesFromElements(
            <Route>
                <Route path ="/" element={<WithNav auth={auth} setAuth={setAuth}/>}>
                    <Route index element={<PlayQuizStartPage/>}/>
                    <Route path="Search" element={null}/>
                    <Route path="Profile" element={<Profil/>}/>
                    <Route path="Leaderboard" element={<Leaderboard/>}/>
                    <Route path="QuizMaker" element={<QuizMaker/>}/>
                    <Route path="Admin" element={<Admin/>}/>
                    <Route path="User/:username" element={<PosetiProfil/>}/>
                    <Route path="ContactUs" element={<ContactUs/>}/>
                </Route> 

                <Route element={<WithoutNav />}>
                    <Route path="Login" element={<Login auth setAuth/>}/>
                    <Route path="Login/ForgotPasswordForm/" element={<ForgotPassword/>}/>
                </Route>
            </Route>
        )
    )
    return(
            <RouterProvider router={router}/> 
    );
}
 
export default Navbar;