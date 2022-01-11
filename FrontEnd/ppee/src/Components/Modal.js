import React, { Component } from 'react';

const powerPlantTypes = ["Coal", "Gas", "Hydropower", "Wind","Solar"]

class Modal extends Component {
    constructor() {
        super();
        this.state = {
            maxProductionValue: 0,
            minProductionValue: 0,
            productionType: 'Coal',
        }
    }

    handleInputChange = event => {
        this.setState({
            [event.target.id]: event.target.value
        })
    }



    handleSubmit = () => {

        console.log(this.state);

        //ako prodje sve kako treba, zatvoriti modal
        //this.props.toggleModal();
    }

    CloseModal = event => {
        //da ne zatvori modal kad se klikne na input polje
        if(event.target.id !== "overlay")
            return;

        this.setState({
            maxProductionValue: 0,
            minProductionValue: 0,
            productionType: undefined,
        })
        this.props.toggleModal();
    }

    render() {
        return (
                <div className='modal-overlay' onClick={this.CloseModal} id="overlay">
                    <div className="modal-form">
                        <h3 className='modal-header'>Generator form</h3>

                        <label className='modal-label' htmlFor="maxProductionValue">Max Production Value</label>
                        <input className='modal-input' type="text" placeholder="max production value" id="maxProductionValue" onChange={this.handleInputChange} />

                        <label className='modal-label' htmlFor="minProductionValue">Min Production Value</label>
                        <input className='modal-input' type="text" placeholder="min production value" id="minProductionValue" onChange={this.handleInputChange} />

                        <label className='modal-label' htmlFor="minProductionValue">Power plant production type</label>
                        <select className='modal-select' id='productionType' value={this.state.productionType} onChange={this.handleInputChange}>
                            {
                                powerPlantTypes.map(item => {
                                    return <option key={item} value={item}>{item}</option>
                                })
                            }
                        </select>

                        <button className='modal-submit' onClick={this.handleSubmit}>Submit</button>
                    </div>
                </div>
        );
    }
}

export default Modal;