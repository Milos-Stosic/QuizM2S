import { useState } from 'react';
import Odgovor from './Odgovor'
const Pitanje = (props) => {
    const id=props.pitanje.id;
    const [odgovori,setOdgovori]=useState(props.pitanje.odgovori);
    const [tezina,setTezina]=useState(0);
    const [value,setValue]=useState();
    
    
    const dodajOdgovor=()=>{
        if(odgovori.length<8){
            setOdgovori([...odgovori,""]);
            props.pitanje.odgovori.push("");
            // console.log(props.pitanje.odgovori);
            // console.log(props.pitanje.odgovori.length);
        }
    }
    const oduzmiOdgovor=()=>{
        if(odgovori.length>4){
            console.log(props.pitanje.odgovori);
            props.setOdgovori(props.pitanje.id, odgovori.filter((o,index)=>index!==odgovori.length-1))
            setOdgovori( odgovori.filter((o,index)=>index!==odgovori.length-1))
            // console.log(odgovori.length);
            console.log(props.pitanje.odgovori);
        }
    }
    const handleChangeOdgovor=(idp,ido,value)=>{
        console.log("ID pitanja",idp,"\nID odgovora:",ido,"\n")
        props.pitanje.odgovori[ido]=value;
        setOdgovori(props.pitanje.odgovori);
        props.setPitanja(props.pitanja.map((pitanje=>{
            if(id===idp){
                return pitanje
            }else return pitanje;
        })))
    }
    const handleChangeTacanOdgovor=(idp,ido)=>{
        props.pitanje.tacanOdg=ido;
        props.setPitanja(props.pitanja.map((pitanje=>{
            if(id===idp){
                return pitanje;
            }else return pitanje;
        })))
    }
    return (  
        <div className="Pitanje">
            <button className="IzbrisiPitanje" onClick={() => props.handleDelete(props.pitanje.id)}>Izbriši</button>
            <div className='divPit'>
                <label htmlFor="pitanje">Pitanje {props.index+1}:</label>
                <input type="text" id={props.pitanje.id} value={props.pitanja[props.index].tekst} onChange={(event)=>{ props.handleChangePitanje(props.index,event.target.value)}}/>
            </div>
            <div className="Tezina">
                <label htmlFor="tezina">Tezina:</label>
                <select id="tezina" onChange={(event)=>{ props.handleChangeTezinaPitanja(props.pitanje.id,event.target.value)}}>
                    <option value="0" key="0">Lako</option>
                    <option value="1" key="1">Srednje</option>
                    <option value="2" key="2">Tesko</option>
                </select>
            </div>
            <div className="odgOkvir">
                <label htmlFor="odgovori">Odgovori: </label>
                <div className="odgovori">
                    {
                        odgovori.map((odg, id)=>(
                            <Odgovor idp={props.pitanje.id} id={id} key={`${props.pitanje.id}+${id}`} handleChangeOdgovor={handleChangeOdgovor} handleChangeTacanOdgovor={handleChangeTacanOdgovor} pitanje={props.pitanje}></Odgovor>
                        ))
                    }
                </div>
            </div>
            <div className="brojacOdgovora">
            <button className="oduzmiOdgovor" onClick={oduzmiOdgovor}>Odgovor -</button>
            <button className="dodajOdgovor" onClick={dodajOdgovor}>Odgovor +</button>
            </div>
        </div>
    );
}
 
export default Pitanje;