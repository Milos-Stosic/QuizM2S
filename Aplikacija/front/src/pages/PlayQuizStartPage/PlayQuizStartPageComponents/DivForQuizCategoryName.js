import "../PlayQuizStartPage.css"

export default function DivForQuizCategoryName(props){
    return(
        <div className="quiz-attributes-div">
            {props.children}
        </div>
    )
}