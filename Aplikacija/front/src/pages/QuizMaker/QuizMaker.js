import { useState } from "react";
import Pitanje from "./Pitanje";
import useFetch from "../../useFetch";
import useCurrentUser from "../../useCurrentUser";

const QuizMaker = () => {
  const {
    data: cats,
    isLoading,
    error,
  } = useFetch("http://localhost:5013/Category/GetCategories");
  const {
    data: trKorisnik,
    isLoading: isLoadingTrKorisnik,
    error: errorTrKorisnik,
  } = useCurrentUser();

  const [temp, setTemp] = useState(0);
  const [naziv, setNaziv] = useState("");
  const [tezina, setTezina] = useState(0);
  const [kategorija, setKategorija] = useState("null");

  const [showMinimumQuestionPopup, setShowMinimumQuestionPopup] =
    useState(false);
  const [showChooseCategoryPopup, setShowChooseCategoryPopup] = useState(false);
  const [showQACantBeEmpty, setShowQACantBeEmpty] = useState(false);
  const [showNazivKvizaErr, setShowNazivKvizaErr] = useState(false);
  const [pitanja, setPitanja] = useState([
    {
      id: 0,
      tekst: "",
      tezina: "0",
      tacanOdg: null,
      odgovori: ["", "", "", ""],
    },
  ]);

  const handleChangePitanje = (id, value) => {
    pitanja[id].tekst = value;
    setPitanja(
      pitanja.map((pitanje) => {
        return pitanje;
      })
    );
  };
  const setOdgovori = (idp, odgovori) => {
    pitanja.map((pitanje) => {
      if (pitanje.id == idp) pitanje.odgovori = odgovori;
    });
  };
  const handleChangeTezinaPitanja = (id, value) => {
    setPitanja(
      pitanja.map((pitanje) => {
        if (pitanje.id === id) {
          return { ...pitanje, tezina: value };
        } else return pitanje;
      })
    );
  };
  const handleDelete = (id) => {
    const novaPitanja = pitanja.filter((pitanje) => pitanje.id !== id);
    console.log(novaPitanja);
    setPitanja(novaPitanja);
  };

  const dodajPitanje = () => {
    if (temp < 19) {
      setTemp(temp + 1);
      setPitanja([
        ...pitanja,
        {
          id: temp + 1,
          tekst: "",
          tezina: "0",
          tacanOdg: null,
          odgovori: ["", "", "", ""],
        },
      ]);
    }
  };
  const checkQuizName = async () => {
    try {
      const response = await fetch(`http://localhost:5013/QuizMaker/CheckIfQuizExist/${naziv}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      
      const text = await response.text();
      console.log(text);
  
      return text;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };
  const dodajKviz = async () => {
    let flag = false;

    pitanja.forEach((pitanje) => {
      console.log(pitanje);
    });
    let div = document.getElementById("QM");
    pitanja.map((pitanje) => {
      if (pitanje.tacanOdg == null) {
        flag = true;
      }
    });

    pitanja.map((pitanje) => {
      pitanje.odgovori.map((odgovor) => {
        if (odgovor.length < 1) {
          flag = true;
        }
      });
      if (showQACantBeEmpty === true) return;
    });
    pitanja.map((pitanje) => {
      if (pitanje.tekst < 1) {
        flag = true;
      }
    });

    const provera=await checkQuizName();

    if(provera==="true"){
      console.log(provera);
      setShowQACantBeEmpty(false);
      setShowChooseCategoryPopup(false);
      setShowMinimumQuestionPopup(false);
      setShowNazivKvizaErr(true);
    }else if (pitanja.length < 5) {
      setShowMinimumQuestionPopup(true);
    } else if (kategorija === "null") {
      setShowChooseCategoryPopup(true);
      setShowMinimumQuestionPopup(false);
    } else if (flag) {
      setShowQACantBeEmpty(true);
      setShowChooseCategoryPopup(false);
      setShowMinimumQuestionPopup(false);
    } else if (naziv.length < 2) {
      setShowQACantBeEmpty(true);
      setShowChooseCategoryPopup(false);
      setShowMinimumQuestionPopup(false);
    } 
    else{
      console.log(naziv, tezina, kategorija, pitanja);
      div.classList.add("noClicks");
      flag = false;
      setShowQACantBeEmpty(false);
      setShowNazivKvizaErr(false);
      setShowChooseCategoryPopup(false);
      setShowMinimumQuestionPopup(false);
      await fetch(
        `http://localhost:5013/QuizMaker/CreateQuiz/${encodeURIComponent(
          naziv
        )}/${tezina}/${kategorija}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      await Promise.all(
        pitanja.map(async (pitanje) => {
          await fetch(
            `http://localhost:5013/QuizMaker/CreateQuestion/${encodeURIComponent(
              pitanje.tekst
            )}/${pitanje.odgovori[pitanje.tacanOdg]}/${
              pitanje.tezina
            }/${kategorija}`,
            {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              },
            }
          );
          pitanje.odgovori
            .filter((odgovor) => {
              return odgovor != pitanje.odgovori[pitanje.tacanOdg];
            })
            .map(
              async (odgovor) =>
                await fetch(
                  `http://localhost:5013/QuizMaker/CreateAnswerAndConnectToQuestion/${odgovor}/${encodeURIComponent(
                    pitanje.tekst
                  )}`,
                  {
                    method: "POST",
                    headers: {
                      "Content-Type": "application/json",
                      Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                  }
                )
            );
          await fetch(
            `http://localhost:5013/QuizMaker/AddQuestionToQuiz/${encodeURIComponent(
              naziv
            )}/${encodeURIComponent(pitanje.tekst)}`,
            {
              method: "PUT",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              },
            }
          );
          console.log(pitanje.tekst);
        })
      )
        .then(() => {
          console.log("The end");
          div.classList.remove("noClicks");
        })
        .catch((error) => {
          console.log(error);
        });
      window.location.reload();
    }
  };

  return error ? (
    <div>{error}</div>
  ) : isLoading ? (
    <div>Pribavljanje Kategorija...</div>
  ) : localStorage.Guest === "true" ? (
    <h1>Nemate pristup stranici za kreatore</h1>
  ) : errorTrKorisnik ? (
    <h1>Doslo je do greske pri ucitavnaju podataka o korisniku</h1>
  ) : isLoadingTrKorisnik ? (
    <h1></h1>
  ) : !trKorisnik ? (
    <h1></h1>
  ) : trKorisnik.role === "QuizMaker" ? (
    <div id="QM" className="QuizMaker">
      <h1>
        Kreiranje kviza
        <p style={{ color: "gray", fontSize: "small" }}>
          (Redosled pitanja i odgovora nije bitan)
        </p>
      </h1>
      <div className="Left">
        <h2>Kviz</h2>
        <div className="NazKatTez">
          <div className="Naziv">
            <label htmlFor="naziv">Naziv Kviza:</label>
            <input
              type="text"
              id="naziv"
              value={naziv}
              onChange={(e) => {
                setNaziv(e.target.value);
              }}
            />
          </div>

          <div className="Tezina">
            <label htmlFor="tezina">Tezina:</label>
            <select
              id="tezina"
              value={tezina}
              onChange={(e) => setTezina(e.target.value)}
            >
              <option value={0} key="0">
                Lako
              </option>
              <option value={1} key="1">
                Srednje
              </option>
              <option value={2} key="2">
                Tesko
              </option>
            </select>
          </div>
          <div className="Kategorija">
            <label htmlFor="kategorija">Kategorija:</label>
            <select
              id="kategorija"
              value={kategorija}
              onChange={(e) => setKategorija(e.target.value)}
            >
              <option value={"null"} key={-1}>
                {"<Odaberite>"}
              </option>
              {cats &&
                cats
                  .filter((cat) => cat.name !== 0)
                  .map((cat, index) => (
                    <option value={cat.name} key={index}>
                      {cat.name}
                    </option>
                  ))}
            </select>
          </div>

          <button className="DodajKviz" onClick={dodajKviz}>
            Dodaj Kviz
          </button>
          {showMinimumQuestionPopup && (
            <div className="popup">
              <p>Kviz mora da ima minimum 5 pitanja.</p>
              <button onClick={() => setShowMinimumQuestionPopup(false)}>
                Zatvori
              </button>
            </div>
          )}
          {showChooseCategoryPopup && (
            <div className="popup">
              <p>Odaberite kategoriju.</p>
              <button onClick={() => setShowChooseCategoryPopup(false)}>
                Zatvori
              </button>
            </div>
          )}
          {showQACantBeEmpty && (
            <div className="popup">
              <p>Popunite sva polja.</p>
              <button onClick={() => setShowQACantBeEmpty(false)}>
                Zatvori
              </button>
            </div>
          )}
          {showNazivKvizaErr && (
            <div className="popup">
              <p>Ime kviza je zauzeto.</p>
              <button onClick={() => setShowNazivKvizaErr(false)}>
                Zatvori
              </button>
            </div>
          )}
        </div>
      </div>

      <div className="Right">
        <div className="Pitanja">
          <h2>Pitanja</h2>
          {pitanja.map((pitanje, index) => (
            <Pitanje
              key={index}
              index={index}
              pitanje={pitanje}
              handleDelete={handleDelete}
              handleChangePitanje={handleChangePitanje}
              handleChangeTezinaPitanja={handleChangeTezinaPitanja}
              pitanja={pitanja}
              setPitanja={setPitanja}
              setOdgovori={setOdgovori}
            ></Pitanje>
          ))}
        </div>
        <button className="DodajPitanje" onClick={dodajPitanje}>
          Dodaj Pitanje
        </button>
      </div>
    </div>
  ) : (
    <h1>Nemate pristup stranici za kreatore</h1>
  );
};
export default QuizMaker;
