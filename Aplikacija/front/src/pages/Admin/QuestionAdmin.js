import React from "react";

const QuestionAdmin = ({ question,index }) => {
  const { id, text, answers, correctAnswer } = question;

  return (
    <div className="questionAdmin">
      <h3>Pitanje {index+1}</h3>
      <p>{text}</p>
      <p>Odgovori:</p>
      <ul>
        <li className="correctA">{correctAnswer}</li>
        {answers.filter(answer=>answer.text!=correctAnswer).map((answer) => (
          <li key={answer.id}>{answer.text}</li>
        ))}
      </ul>
    </div>
  );
};

export default QuestionAdmin;