import { useState, useEffect } from "react";

const useCurrentUser=()=>{
  const [data, setData]=useState();
  const [isLoading,setisLoading]=useState(true);
  const [error,setError]=useState();

  useEffect(()=>{
    async function runFetch(){
      fetch(`http://localhost:5013/Login/UsersEndpoint`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
    })
    .then(response=>{
      if(!response.ok){
        throw Error("Nastala je greska pri nabavljanju podataka");
      }
      return response.json();
    })
    .then((data)=>{
      setData(data);
      setisLoading(false);
      setError(null);
    })
    .catch(e=>{
      setisLoading(false);
      setError(e.message);
      console.log(error);
    })
  }
  if(!data && localStorage.getItem("token")){
  runFetch();
    // },1000)
}
},[data]);

return {
  data,
  isLoading,
  error
}
}
export default useCurrentUser;