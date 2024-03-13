const Odgovor = props => {
    return (
        <div className="odgovor" key={props.id}>
            <label className="radio"></label>
            <input type="radio" name={props.idp} value={props.idp + props.id} onChange={(e)=>props.handleChangeTacanOdgovor(props.idp,props.id)} id="radio"/>
            <input type="text" className = "odg" id = { props.idp + props.id } value={props.pitanje.odgovori[props.id]} onChange={ev=>{props.handleChangeOdgovor(props.idp,props.id,ev.target.value)}}/>
        </div>
      );
}
export default Odgovor;