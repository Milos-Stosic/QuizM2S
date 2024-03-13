import { useState, useEffect } from "react";

const useFetch = (url) =>  {

    const [data, setData]=useState();
    const [isLoading,setisLoading]=useState(true);
    const [error,setError]=useState();

    useEffect(()=>{
        const abortCont = new AbortController();
    // setTimeout(()=>{
        async function runFetch(){
            fetch(url,{
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                  },
                signal:abortCont.signal})
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
            if(e.name ==="AbortError"){
            }else
            {
                setisLoading(false);
                setError(e.message);
                console.log(error);
            }
            }) 
        }
        
        runFetch();
        return () => abortCont.abort();
    // },1000)
},[url,error]);
return {
    data,
    isLoading,
    error
}
}
export default useFetch;