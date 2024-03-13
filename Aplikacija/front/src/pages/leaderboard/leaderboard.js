import "./leaderboardStil.css";
import useFetch from "../../useFetch";
import { useNavigate } from "react-router-dom";
function Leaderboard() {
  const navigate=useNavigate();
  const { data: users, isLoading, error } = useFetch('http://localhost:5013/Score/Leaderboard');
  const handleVisitProfile=(username)=>{
    navigate(`/User/${username}`);
  }
  return (
    <div className="lista">
      <h2 className="listaR">Rang Lista</h2>
      <>
      {isLoading && <div>Ucitavanje...</div>}
      {error && <div>{error}</div>}
      {users && users.filter(m=>m.name!=="Random").map((m, index) => (
        <div className="redovi" key={index} onClick={()=>{handleVisitProfile(m.name)}}>
          <div className="rBr">
            <label className="red">{++index}.</label>
          </div>
          <div className="nickL">
            <label className="red">{m.name}</label>
          </div>
          <div className="nickL">
            <label className="red">Odigrao: {m.numberOfQuizzesPlayed}</label>
          </div>
          <div className="nickL">
            <label className="red">Poena: {m.score}</label>
          </div>
        </div>
      ))}
      </>
    </div>
  );
}

export default Leaderboard;
