import DivForQuizzes from "./PlayQuizStartPageComponents/DivForQuizzes";
import MainDiv from "./PlayQuizStartPageComponents/MainDiv";
import OptionsDiv from "./PlayQuizStartPageComponents/OptionsDiv";
import "./PlayQuizStartPage.css";
import DivForQuizName from "./PlayQuizStartPageComponents/DivForQuizName";
import DivForQuizDifficultyName from "./PlayQuizStartPageComponents/DivForQuizDifficultyName";
import DivForCategoryName from "./PlayQuizStartPageComponents/DivForQuizCategoryName";
import DivForSelect from "./PlayQuizStartPageComponents/DivForSelect";
import DivForButton from "./PlayQuizStartPageComponents/DivForButton";
import useFetch from "../../useFetch";
import { useState, useEffect } from "react";
import ReadyForQuizPage from "../ReadyForQuizPage/ReadyForQuizPage";

const PlayQuizStartPage = () => {
  const [quizzes, setQuizzes] = useState();
  const [loading, setLoading] = useState(false);
  const [category, setCategory] = useState();

  const [difficulty, setDifficulty] = useState();

  const [clicked, setClicked] = useState(false);

  const [idRandomQuiz, setIdRandomQuiz] = useState();

  const [randomClicked, setRandomClicked] = useState(false);

  const [clickedQuiz, setClickedQuiz] = useState();

  const [errorMessage, setErrorMessage] = useState("");

  const [average, setAverage] = useState({});

  const { data: allQuizzes } = useFetch(
    "http://localhost:5013/Quiz/GetQuizzes"
  );

  const {
    data: popularQuizzes,
    isLoadingQuiz,
    errorQuiz,
  } = useFetch("http://localhost:5013/Quiz/GetQuizzesByPopularity");

  const {
    data: categories,
    isLoadingCat,
    errorCat,
  } = useFetch("http://localhost:5013/Category/GetCategories");

  useEffect(() => {
    setQuizzes(popularQuizzes);
    console.log(popularQuizzes);
  }, [popularQuizzes]);

  useEffect(() => {
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
    }, 500);
  }, []);

  useEffect(() => {
    if (quizzes !== undefined) if (quizzes.length > 0) {
      quizzes.map((q)=>(
        fetch(`http://localhost:5013/Quiz/GetAverageRating/${q.title}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        })
          .then((res) => {
            if (!res.ok) {
              console.log(res.text());
            } else {
              res.text().then((r) => {
                console.log(r);
                if(r!==undefined && r!=="NaN"){
                  setAverage((prevAvgRatings) => ({
                    ...prevAvgRatings,
                    [q.title]: r,
                  }));
                }
                else{
                  setAverage((prevAvgRatings) => ({
                    ...prevAvgRatings,
                    [q.title]: "",
                  }));
                }
              });
            }
          })
          .catch((err) => {
            console.log(err);
            setAverage((prevAvgRatings) => ({
              ...prevAvgRatings,
              [q.title]: null,
            }));
          })
      ));
    }
  }, [quizzes]);

  const handleOnClick = (quiz) => {
    setClicked(true);
    setClickedQuiz(quiz);
    console.log(quiz);
  };


  const handleRandomOnClick = () => {
    let dif;
    console.log(difficulty);
    if (difficulty === "lako") dif = 0;
    else if (difficulty === "srednje") dif = 1;
    else if (difficulty === "tesko") dif = 2;
    else {
      dif = undefined;
    }

    fetch(`http://localhost:5013/Quiz/GenerateQuizOfDifficulty/${dif}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((res) => {
        if (!res.ok) {
          res.text().then((text) => {
            console.log(text);
            setErrorMessage("Odaberite tezinu nasumicnog kviza");
          });
        } else {
          console.log(res);
          res.json().then((text) => {
            console.log(text);
            setIdRandomQuiz(text.id);
            setRandomClicked(true);
            setClickedQuiz(text);
            setErrorMessage("");
          });
        }
      })
      .catch((err) => {
        console.log(err);
      });
  };

  if (randomClicked === true) {
    return <ReadyForQuizPage quiz={clickedQuiz}></ReadyForQuizPage>;
  }

  if (clicked === true) {
    return <ReadyForQuizPage quiz={clickedQuiz} />;
  }

  const handleOnChangeCategory = (event) => {
    setCategory(event.target.value);
    showQuizzes(event.target.value, difficulty);
  };

  const handleOnChangeDifficulty = (event) => {
    setDifficulty(event.target.value);
    setErrorMessage("");
    showQuizzes(category, event.target.value);
  };

  const showQuizzes = (category, difficulty) => {
    setCategory(category);
    setDifficulty(difficulty);
    setErrorMessage("");

    console.log(difficulty);
    if (category === undefined) {
      category = "";
      setCategory("");
    }

    if (difficulty === undefined) {
      difficulty = "";
      setDifficulty("");
    }

    console.log(category);
    console.log(difficulty);

    if (category === "" && difficulty === "") {
      const popQuizzes = popularQuizzes.filter(
        (quiz) => quiz.category !== "Random"
      );
      setQuizzes(popQuizzes);
    }

    if (
      (category === "Odaberite kategoriju" || category === "") &&
      (difficulty === "Odaberite tezinu" || difficulty === "")
    ) {
      const popQuizzes = popularQuizzes.filter(
        (quiz) => quiz.category !== "Random"
      );
      console.log(popQuizzes);
      setQuizzes(popQuizzes);
    }

    if (
      category !== "Odaberite kategoriju" &&
      category !== "" &&
      (difficulty === "Odaberite tezinu" || difficulty === "")
    ) {
      console.log("heeej");
      const quizzesOfCategory = allQuizzes.filter(
        (quiz) => quiz.category === category && quiz.category !== "Random"
      );
      setQuizzes(quizzesOfCategory);
    }

    if (
      (category === "" || category === "Odaberite kategoriju") &&
      difficulty !== "Odaberite tezinu" &&
      difficulty !== ""
    ) {
      const quizzesOfDifficulty = allQuizzes.filter(
        (quiz) => quiz.difficulty === difficulty && quiz.category !== "Random"
      );
      setQuizzes(quizzesOfDifficulty);
    }

    if (
      category !== "Odaberite kategoriju" &&
      category !== "" &&
      difficulty !== "Odaberite tezinu" &&
      difficulty !== ""
    ) {
      const quizzesOfDiffAndCategory = allQuizzes.filter(
        (quiz) =>
          quiz.category === category &&
          quiz.category !== "Random" &&
          quiz.difficulty === difficulty
      );
      setQuizzes(quizzesOfDiffAndCategory);
    }
  };

  return (
    <div className="whole-page-div">
      {loading ? (
        <div className="loader-container"></div>
      ) : (
        <MainDiv>
          <div className="overflow-div">
            {errorQuiz && <div>{errorQuiz}</div>}
            {isLoadingQuiz && <div>Ucitavanje...</div>}
            {quizzes && quizzes.length !== 0 ? (
              <DivForQuizzes>
                {quizzes
                  .filter((quiz) => quiz.category !== "Random")
                  .map((pQuiz, index) => (
                    <div
                      className="quiz-div"
                      key={index}
                      onClick={() => handleOnClick(pQuiz)}
                    >
                      <DivForQuizName>{pQuiz.title}</DivForQuizName>
                      <DivForQuizDifficultyName>
                        {pQuiz.difficulty === "lako"
                          ? "Lak"
                          : pQuiz.difficulty === "srednje"
                          ? "Srednji"
                          : "Težak"}
                      </DivForQuizDifficultyName>
                      <DivForCategoryName>{pQuiz.category}</DivForCategoryName>
                      <div className="rating-div">
                        Ocena: {average[pQuiz.title]}/5
                      </div>
                    </div>
                  ))}
              </DivForQuizzes>
            ) : (
              <div className="nema-kvizova-div">Nema kvizova za prikaz</div>
            )}
          </div>
          <OptionsDiv>
            <DivForSelect>
              {errorCat && <div>{errorCat}</div>}
              {isLoadingCat && <div>Učitavanje...</div>}
              {categories && (
                <>
                  <select
                    className="select-category"
                    onChange={handleOnChangeCategory}
                  >
                    <option defaultValue="">Odaberite kategoriju</option>
                    {categories
                      .filter((cats) => cats.name !== "Random")
                      .map((cat, index) => (
                        <option value={cat.name} key={index}>
                          {cat.name}
                        </option>
                      ))}
                  </select>
                </>
              )}
            </DivForSelect>
            <DivForSelect>
              <select
                className="select-difficulty"
                onChange={handleOnChangeDifficulty}
              >
                <option defaultValue="">Odaberite tezinu</option>
                <option value="lako">Lako</option>
                <option value="srednje">Srednje</option>
                <option value="tesko">Teško</option>
              </select>
            </DivForSelect>
            <DivForButton>
              <button className="button-random" onClick={handleRandomOnClick}>
                Nasumican Kviz
              </button>
              <div className="error">{errorMessage}</div>
            </DivForButton>
          </OptionsDiv>
        </MainDiv>
      )}
    </div>
  );
};

export default PlayQuizStartPage;
