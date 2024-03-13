import Question from "./Components/Question";
import "../StartQuizPage/StartQuizPage.css"

export default function StartQuizPage(props){

    return(
        <div className="whole-page">
            <h1>{props.quiz.title}</h1>
            <div className="pitanje-odgovor-div">
                <Question quizTitle={props.quiz.title}/>
            </div>
        </div>
    )
}