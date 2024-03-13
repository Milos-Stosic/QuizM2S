import "../PlayQuizStartPage.css"

export default function DivForQuizDifficultyName(props){
    return(
        <div className="quiz-attributes-div">
            {props.children}
        </div>
    )
}