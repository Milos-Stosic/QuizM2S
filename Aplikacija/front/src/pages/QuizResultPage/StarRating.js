import React, { useState, useEffect } from "react";

const StarRating = (props) => {
  const [rating, setRating] = useState(0);
  const [hover, setHover] = useState(0);
  const [rated, setRated] = useState(false);
  const [disabled, setDisabled] = useState(false);
  const [message, setMessage] = useState(false);

  const handleRated = (index) => {
    setRating(index);
    setRated(true);
    setDisabled(true);
    setMessage(true);
  };

  useEffect(() => {
    if (rated === true) {
      fetch(
        `http://localhost:5013/Quiz/AddRatingToQuiz/${rating}/${props.quiz}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      )
        .then((res) => {
          if (!res.ok) {
            console.log("Greska kod ocenjivanja kviza");
          } else {
            console.log("Uspesno ocenjen kviz");
          }
        })
        .catch((err) => {
          console.log(err);
        });
    }
  }, [rated]);

  return (
    <div className="star-rating">
      {disabled === true ? (
        <span className="obavestenje">Uspešno ste ocenili ovaj kviz</span>
      ) : (
        [...Array(5)].map((star, index) => {
          index += 1;
          return (
            <div
              type="input"
              key={index}
              className={index <= ((rating && hover) || hover) ? "on" : "off"}
              onClick={() => {
                handleRated(index);
              }}
              onMouseEnter={() => setHover(index)}
              onMouseLeave={() => setHover(rating)}
            >
              <span className="star">&#9733;</span>
            </div>
          );
        })
      )}
    </div>
  );
};

export default StarRating;
