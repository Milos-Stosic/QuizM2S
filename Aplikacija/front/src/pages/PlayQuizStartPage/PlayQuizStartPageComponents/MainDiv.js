import "../PlayQuizStartPage.css";

export default function MainDiv(props){
    return(
        <div className="main-div">
            {props.children}
        </div>
    )
}