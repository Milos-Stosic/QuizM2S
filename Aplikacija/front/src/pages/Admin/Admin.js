import { useState } from 'react';
import useCurrentUser from "../../useCurrentUser";
import '../../css/admin.css';
import useFetch from "../../useFetch";
import QuestionAdmin from './QuestionAdmin';

const Admin = () => {
    
    const {data:trKorisnik,isLoading: isLoadingTrKorisnik,error: errorTrKorisnik} =  useCurrentUser();

    const {data : pendingKvizovi, isLoading : kvizoviLoading, error : kvizoviError} = useFetch('http://localhost:5013/Admin/GetPendingQuizzes')
    const {data : pendingQM, isLoading : qmLoading, error : qmError}=useFetch("http://localhost:5013/Admin/QuizMakerRequests")
    const {data : pendingA, isLoading : aLoading, error : aError}=useFetch("http://localhost:5013/Admin/GetAdminRequests")
   
    const containerClick=(index)=>{

        let rowKorisnici=document.getElementById("korisnici");
        let rowKvizovi=document.getElementById("kvizovi");

        let divKorisnici=document.getElementById("divKorisnici");
        let divKvizovi=document.getElementById("divKvizovi");

        if(index==="korisnici"){
            rowKorisnici.classList.add("show");
            rowKvizovi.classList.remove("show");
            divKorisnici.classList.add("kliknut");
            divKvizovi.classList.remove("kliknut");
        }else{
            rowKorisnici.classList.remove("show");
            rowKvizovi.classList.add("show");
            divKorisnici.classList.remove("kliknut");
            divKvizovi.classList.add("kliknut");
        }
        
    };

    const handleClick=(index)=>{
        let rowKviz=document.getElementById(index)
        if(rowKviz.classList.contains("show")){
            rowKviz.classList.remove("show");
            return;
        }
        console.log(pendingKvizovi[index]);
        rowKviz.classList.add("show");
    }
    const handleClickKorisnik=(index)=>{
        let rowKviz=document.getElementById(index)
        if(rowKviz.classList.contains("show")){
            rowKviz.classList.remove("show");
            return;
        }
        rowKviz.classList.add("show");
    }
    const odobriKviz=async(index)=>{
        console.log(pendingKvizovi[index].title);
        await fetch(`http://localhost:5013/Admin/AcceptQuiz/${pendingKvizovi[index].title}`,{
            method:'PUT',
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              }}
        ).then(
            console.log("Kviz je odobren"),
            )
            window.location.reload()
    }
    const izbrisiKviz=async(index)=>{
        await fetch(`http://localhost:5013/Admin/DeleteQuiz/${pendingKvizovi[index].title}`,{
            method:'DELETE',
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              }}
        ).then(
            console.log("Kviz je izbrisan"),
            )
            window.location.reload()
    }

    const odobri=async(index,role)=>{
        await fetch(`http://localhost:5013/Admin/RoleChange/${index}/${role}`,{
            method:'PUT',
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              }}
        ).then(
            console.log("Korisnik je odobren"),
            )
            window.location.reload()
    }

    const odbij=async(index)=>{
        await fetch(`http://localhost:5013/Admin/RejectAdminRequest/${index}`,{
            method:"PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              }}
        ).then(
            console.log("Korisnik je odbijen"),
            )
            window.location.reload()
    }

    const odbijKreatora=async(index)=>{
        await fetch(`http://localhost:5013/Admin/RejectQMRequest/${index}`,{
            method:"PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              }}
        ).then(
            console.log("Korisnik je odbijen"),
            )
            window.location.reload()
    }


    return (
        <div className="adminContainer">
            {localStorage.Guest=="true" ? <h1>Nemate pristup admin stranici</h1> : errorTrKorisnik ? <h1>Doslo je do greske pri ucitavnaju podataka o korisniku</h1> : isLoadingTrKorisnik ? <h1></h1> : !trKorisnik ? <h1></h1>     : trKorisnik.role=="Admin" ? 
            <div className='adminContainerUnutrasnji'>
                <h1 className="naslov">
                    Admin
                </h1>
                <div className="rowOne">
                    <div className="smallContainer" id="divKvizovi" onClick={()=>containerClick("kvizovi")}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="white" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9 5.25h.008v.008H12v-.008z" />
                        </svg>
                        <label className='opis'>
                            Kvizovi
                        </label>
                    </div>
                    <div className="smallContainer" id="divKorisnici" onClick={()=>containerClick("korisnici")}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="white" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M18 18.72a9.094 9.094 0 003.741-.479 3 3 0 00-4.682-2.72m.94 3.198l.001.031c0 .225-.012.447-.037.666A11.944 11.944 0 0112 21c-2.17 0-4.207-.576-5.963-1.584A6.062 6.062 0 016 18.719m12 0a5.971 5.971 0 00-.941-3.197m0 0A5.995 5.995 0 0012 12.75a5.995 5.995 0 00-5.058 2.772m0 0a3 3 0 00-4.681 2.72 8.986 8.986 0 003.74.477m.94-3.197a5.971 5.971 0 00-.94 3.197M15 6.75a3 3 0 11-6 0 3 3 0 016 0zm6 3a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0zm-13.5 0a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z" />
                        </svg>
                        <label className='opis'>
                            Korisnici
                        </label>
                    </div>
                </div>
                <div id="kvizovi" className="rowKvizovi">
                    {(kvizoviError || pendingKvizovi===undefined) && <div>Nema kvizova za prikaz</div>}
                    {kvizoviLoading && <div>Ucitavanje...</div>}
                    { 
                        pendingKvizovi && pendingKvizovi.map((kviz, index)=>{
                            return(
                                <div  className='KvizA'  key={index}>
                                    <div className="kvizOsnovno" onClick={()=>{handleClick(index)}}>
                                        <div className='divX'>
                                            <div style={{ maxWidth: '250px', width:'50%' }}>
                                                <p style={{ wordWrap: 'break-word' }}> {kviz.title}</p>
                                            </div>
                                            <div>
                                                <p>Težina: {kviz.difficulty === "0" ? "Lako" : kviz.difficulty === "1" ? "Srednje" : "Teško" }</p>
                                                <p>Kategorija: {kviz.category.name}</p>
                                            </div>
                                        </div>
                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                                <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                                        </svg>
                                    </div>
                                    <div className="podaci" id={index}>
                                        <div className='PitanjaAdmin'>
                                            {
                                                kviz.questions.length<1 && <div>Nema pitanja</div>
                                            }
                                            {
                                                kviz.questions.map((question, index) => (
                                                    <QuestionAdmin key={question.id} question={question} index={index}/>
                                                ))
                                            }
                                        </div>
                                        <div className="buttonsAdmin">
                                            <button onClick={()=>{odobriKviz(index)}}>Odobri kviz</button>
                                            <button onClick={()=>{izbrisiKviz(index)}}>Izbriši kviz</button>
                                        </div>
                                    </div>
                                </div>
                            )
                        })
                    }
                </div>
                <div id="korisnici" className="rowKorisnici">
                    <div className="kvizOsnovno" onClick={()=>{handleClickKorisnik("kreatori")}} >
                        <label>Zahtevi za kreatore</label>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                        </svg>
                        
                    </div>
                        <div className="podaci" id="kreatori">
                            {(qmError || pendingQM===undefined) && <div style={{alignItems:'center'}}>Nema korisnika koji žele da budu kreatori kviza</div>}
                            {qmLoading && <div>Loading...</div>}
                            {
                                pendingQM && pendingQM.map((qm,index)=>{
                                    return(
                                        <div key={index} className='korisnikA'>
                                            <div className='korisnikApodaci'>
                                                <p>{qm.name}</p>
                                                <p>{qm.email}</p>
                                                <p>{qm.role=="User" ? "Korisnik" : qm.role=="Admin" ? "Admin" : qm.role=="QuizMaker" ? "Kreator" : "Gost"}</p>
                                            </div>
                                            <div className="buttonsAdmin">
                                                <button onClick={()=>{odobri(qm.id,"QuizMaker")}}>Odobri kreatora</button>
                                                <button onClick={()=>{odbijKreatora(qm.id)}}>Odbij kreatora</button>
                                            </div>
                                        </div>
                                    )
                                })
                            }
                        </div>
                        
                        <div  className="kvizOsnovno" onClick={()=>{handleClickKorisnik("admini")}} style={{borderTop:'1px solid white'}}>
                            <label>Zahtevi za admine</label>
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                                            <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 8.25l-7.5 7.5-7.5-7.5" />
                            </svg>
                        </div>
                        <div className="podaci" id="admini">
                            {(aError || pendingA===undefined) && <div>Nema zahteva za ulogu admina</div>}
                            {aLoading && <div>Ucitavanje...</div>}
                            {
                                pendingA && pendingA.map((admin,index)=>{
                                    return(
                                        <div key={index} className='korisnikA'>
                                            <div className='korisnikApodaci'>
                                                <p>{admin.name}</p>
                                                <p>{admin.email}</p>
                                                <p>{admin.role=="User" ? "Korisnik" : admin.role=="Admin" ? "Admin" : admin.role=="QuizMaker" ? "Kreator" : "Gost"}</p>
                                            </div>
                                            <div className="buttonsAdmin">
                                                <button onClick={()=>{odobri(admin.id,"Admin")}}>Odobri admina</button>
                                                <button onClick={()=>{odbij(admin.id)}}>Odbij admina</button>
                                            </div>
                                        </div>
                                    )
                                })
                            }
                        </div>
                        
                </div>
            </div> : <h1>Nemate pristup admin stranici</h1>}
            
        </div>
    )
}
export default Admin;