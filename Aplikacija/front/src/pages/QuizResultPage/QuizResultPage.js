import { useEffect, useState } from "react";
import ReadyForQuizPage from "../ReadyForQuizPage/ReadyForQuizPage";
import "../QuizResultPage/QuizResultPage.css";
import useCurrentUser from "../../useCurrentUser";
import StarRating from "./StarRating";

export default function QuizResultPage(props) {
  const [isUnauthorized, setIsUnauthorized] = useState(false);
  const [score, setScore] = useState();
  const [isClicked, setIsClicked] = useState(false);
  const { data: currentUser } = useCurrentUser();
  const [show, setShow] = useState(false);
  const [clicked, setClicked] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!localStorage.getItem("token")) {
      setIsUnauthorized(true);
    }
  }, []);

  const getResults = async () => {
    setIsClicked(true);
  };

  const showStars = () => {
    setShow(true);
    setClicked(true);
    if (clicked === true) {
      setShow(false);
      setClicked(false);
    }
  };

  const handleOnClick = () => {
    window.location.reload(false);
  };

  return (
    <div className="div-result">
      <div className="rezultatDiv">
        <h2 className="h2-result">Broj tačnih odgovora koje ste dali je:</h2>
        <div className="p-result">{props.score / 10}</div>
      </div>

      <div className="button-result-div">
        <button className="button-result" onClick={handleOnClick}>
          Igraj drugi kviz
        </button>
        {isUnauthorized === false ? (
          <>
            {!loading && (
              <button className="button-result" onClick={getResults}>
                Prikaži tabelu
              </button>
            )}
            {loading && (
              <button className="button-result" onClick={getResults}>
                Učitavanje...
              </button>
            )}
          </>
        ) : (
          ""
        )}
        <button className="button-result" onClick={showStars}>
          Oceni kviz
        </button>
        {show === true ? <StarRating quiz={props.quiz.quizTitle} /> : ""}
      </div>
      {isClicked === true ? (
        <div className="table-div">
          <table className="table">
            <thead>
              <tr>
                <th>Ime</th>
                <th>Rezultat</th>
              </tr>
            </thead>
            <tbody>
              {props.rezultL ?<tr> <td style={{textAlign:"end"}}>Učitavanje...</td> </tr>: props.result ? props.result.map((sc, index) => (
                <tr
                  key={sc.userID.id}
                  className={
                    sc.userID.email === currentUser.email
                      ? "current-user"
                      : "not-current-user"
                  }
                >
                  <td>{sc.userID.name}</td>
                  <td>{sc.scoreValue}</td>
                  </tr>
                  )):<tr><td style={{textAlign:"end"}}>Učitavanje...</td></tr>}
            </tbody>
          </table>
        </div>
      ) : (
        ""
      )}
    </div>
  );
}
