import "../PlayQuizStartPage.css"

export default function DivForQuizName(props){
    return(
        <div className="quiz-names-div">
            {props.children}
        </div>
    )
}