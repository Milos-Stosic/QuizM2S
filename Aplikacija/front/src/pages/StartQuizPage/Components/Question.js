import useFetch from "../../../useFetch";
import { useEffect, useState } from "react";
import "../StartQuizPage.css";
import QuizResultPage from "../../QuizResultPage/QuizResultPage";

export default function Question(props) {
  const {
    data: quiz,
    isLoading,
    error,
  } = useFetch(
    `http://localhost:5013/Question/GetQuestionsForQuiz/${props.quizTitle}`
  );

  const [questions, setQuestions] = useState();

  const [isClicked, setIsClicked] = useState(false);

  const [endOfQuiz, setEndOfQuiz] = useState(false);

  const [currentQuestion, setCurrentQuestion] = useState(0);

  const [answerText, setAnswerText] = useState();

  const [isCorrectAnswer, setIsCorrectAnswer] = useState("");

  const [isChosenAnswer, setIsChosenAnswer] = useState("");

  const [isCorrect, setIsCorrect] = useState(false);

  const [score, setScore] = useState(0);

  const [result, setResult] = useState(0);
  const [resultL, setResultL] = useState(true);

  const [index, setIndex] = useState();

  const [IsUnauthorized, setIsUnauthorized] = useState(false);

  const [timeLeft, setTimeLeft] = useState(10);

  const [shuffleFlag,setShuffleFlag] = useState(false);
  useEffect(() => {
    console.log(quiz);
    if (quiz && !shuffleFlag) {
      const shuffledQuestions = quiz.questions.map((question) => {
        const shuffledAnswers =  shuffleArray(question.answers);
        return { ...question, answers: shuffledAnswers };
      });
      console.log(shuffledQuestions);
      setQuestions(shuffledQuestions);
      setShuffleFlag(true);
    }
  }, [quiz]);

  const shuffleArray = (array) => {
    const newArray = [...array];
    for (let i = newArray.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [newArray[i], newArray[j]] = [newArray[j], newArray[i]];
    }

    return newArray;
  };

  useEffect(() => {
    let timer = null;
   // console.log(isLoading);
    if (!isLoading) {
      if (timeLeft === 0) {
        setIsClicked(true);
        setTimeLeft(null);
      }
      timer =
        timeLeft !== null && setInterval(() => setTimeLeft(timeLeft - 1), 1000);
        console.log("tajmer" + timer);
    }

    return () => clearInterval(timer);
  }, [timeLeft,quiz]);

  const handleAnswerOnClick = (answer, ind) => {
    setIndex(ind);
    setAnswerText(answer);

    setIsChosenAnswer(answer);
    setIsCorrectAnswer(questions[currentQuestion].correctAnswer);

    const chosen = answer;
    const correct = questions[currentQuestion].correctAnswer;

    console.log(chosen);
    console.log(correct);

    if (chosen === correct) {
      setIsCorrect(true);
      setScore(score + 10);
    } else {
      setIsCorrect(false);
    }

    setIsClicked(true);
    setTimeLeft(null);
  };

  const handleNextQuestionOnClick = async () => {
    setCurrentQuestion(currentQuestion + 1);
    setIsClicked(false);
    setIsCorrectAnswer("");
    setIsChosenAnswer("");
    setIsCorrect(false);
    setIndex(null);
    setTimeLeft(10);

    if (currentQuestion >= questions.length - 1) {
      setResultL(true);
      setEndOfQuiz(true);
      setTimeLeft(0);
      try {
        await fetch(
          `http://localhost:5013/Score/AddScoreToUser/${score}/${props.quizTitle}`,
          {
            method: "PUT",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          }
        ).then((res) => {
          console.log(res);
          if (!res.ok) {
            throw res;
          }
        });

        await fetch(
          `http://localhost:5013/Score/GetAllScoresOfQuiz/${props.quizTitle}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          }
        )
          .then((res) => {
            if (res.status === 401 || !res.ok) {
              console.log(res);
              setIsUnauthorized(true);
              throw res;
            } else {
              console.log(res);
              res.json().then((sc) => {
                console.log(sc);
                console.log(sc.map((value) => value.scoreValue));
                setResult(sc);
              });
            }
          })
          .catch((err) => {
            console.log(err);
          });
      } catch (error) {
        console.log(error);
      }
      setResultL(false);
    }
  };

  if (endOfQuiz === true) {
    return (
      <QuizResultPage
        quiz={props}
        resultL={resultL}
        score={score}
        result={result}
        unauthorized={IsUnauthorized}
      />
    );
  }
  return (
    <>
      <div
        className="time-left"
        style={{ width: `${(timeLeft / 10) * 100}%` }}
      />
      <div className="question">
        {error && <div>{error}</div>}
        {isLoading && <div>Ucitavanje...</div>}
        {questions && (
          <div>
            <h2 className="pitanje-text">{questions[currentQuestion].text}</h2>
          </div>
        )}
      </div>
      <div className="answers-div">
        {questions &&(
          <>
            {
              questions[currentQuestion].answers.map((answer, ind) => (
              <div
                onClick={() => handleAnswerOnClick(answer.text, ind)}
                key={ind}
                className={`${
                  isChosenAnswer === answer.text
                    ? isCorrect
                      ? "correct"
                      : "incorrect"
                    : "answer"
                }`}
                style={{ pointerEvents: isClicked ? "none" : "auto" }}>
                <div className="answer-text">{answer.text}</div>
              </div>
            ))}
          </>
        )}
      </div>
      {isClicked === true && (
        <div className="button-next-div">
          <button className="next-button" onClick={handleNextQuestionOnClick}>
            Sledece
          </button>
        </div>
      )}
    </>
  );
}
