import React from "react";
import "../LoginStyles.css"

const LeftOverlayPanel=(props)=>{
    const isTrue = props.signingIn;
    const className= !isTrue ? 'left-panel-transform' : 'left-panel';

    return(
        <div className={className}>
            {props.children}
        </div>
    )
}

export default LeftOverlayPanel;