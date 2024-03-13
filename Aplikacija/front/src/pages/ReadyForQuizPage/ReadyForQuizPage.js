import "../ReadyForQuizPage/ReadyForQuizPage.css";
import { useState } from "react";
import StartQuizPage from "../StartQuizPage/StartQuizPage";

export default function ReadyForQuizPage(props) {
  
  const [daClicked, setDaClicked]= useState(false);

  const handleNeClick = () => {
    window.location.reload(false);
  };

  const handleDaClick=()=>{
    setDaClicked(true);
  }

  if(daClicked===true){
    return(
        <StartQuizPage quiz={props.quiz}/>
    )
  }

  return (
    <div className="whole-page-div">
      <div className="ready-div">
        <h1>JESTE LI SPREMNI DA POCNETE KVIZ?</h1>
        <br />
        <button className="button-ready-da" onClick={handleDaClick}>
            Da
        </button>
        <button className="button-ready-ne" onClick={handleNeClick}>
            Ne
        </button>
      </div>
    </div>
  );
}
