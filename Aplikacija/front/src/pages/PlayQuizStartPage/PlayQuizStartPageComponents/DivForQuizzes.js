import "../PlayQuizStartPage.css";
import slika from "./aqua.png";

export default function DivForQuizzes(props){
    return(
        <div className="quizzes-div">
            <img className="demo-bg"  src={slika} alt=""/>
            {props.children}
        </div>
    )
}