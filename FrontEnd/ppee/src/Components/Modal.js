import React, { Component } from 'react';
import { baseUrl } from '../constants';
import axios from 'axios';

const powerPlantTypes = ["Coal", "Gas", "Hydro", "Wind", "Solar"]

class Modal extends Component {
    constructor() {
        super();
        this.state = {
            type: "Coal",
            maximumOutputPower: 0,
            minimumOutputPower: 0,
            name: "",
            efficiency: 0,
            surfaceArea: 0,
            bladesSweptAreaDiameter: 10,
            numberOfWindGenerators: 10,
            firstPlaceHolder: "Max output power",
            secondPlaceHolder: "Min output power",
            minPowerDisabled: false,
        }
    }

    handleInputChange = event => {

        if (event.target.id === "type") {
            if (event.target.value === "Wind") {
                this.setState({
                    type: event.target.value,
                    firstPlaceHolder: "Blades swept area diameter",
                    secondPlaceHolder: "Number of wind generators",
                    minPowerDisabled: false,
                })
            }
            else if (event.target.value === "Solar") {
                this.setState({
                    type: event.target.value,
                    firstPlaceHolder: "Surface area",
                    secondPlaceHolder: "Efficiency",
                    minPowerDisabled: false,
                })
            }
            else if (event.target.value === "Hydro") {
                this.setState({
                    type: event.target.value,
                    firstPlaceHolder: "Max output power",
                    minPowerDisabled: true,
                })
            }
            else {
                this.setState({
                    minPowerDisabled: false,
                    firstPlaceHolder: "Max output power",
                    secondPlaceHolder: "Min output power",
                })
            }
        }

        this.setState({ [event.target.id]: event.target.value })

    }



    handleSubmit = () => {

        var powerPlant = {
            type: "",
            maximumOutputPower: 0,
            minimumOutputPower: 0,
            name: "",
            efficiency: 0,
            surfaceArea: 0,
            bladesSweptAreaDiameter: 0,
            numberOfWindGenerators: 0,
        }

        if (this.state.type === "Wind") {
            powerPlant.bladesSweptAreaDiameter = this.state.maximumOutputPower;
            powerPlant.numberOfWindGenerators = this.state.minimumOutputPower;
            powerPlant.type = "wind";
            powerPlant.name = this.state.name;
        }
        else if (this.state.type === "Solar") {
            powerPlant.surfaceArea = this.state.maximumOutputPower;
            powerPlant.efficiency = this.state.minimumOutputPower;
            powerPlant.type = "solar";
            powerPlant.name = this.state.name;
        }
        else if (this.state.type === "Hydro") {
            powerPlant.maximumOutputPower = this.state.maximumOutputPower;
            powerPlant.minimumOutputPower = 0;
            powerPlant.type = "hydro";
            powerPlant.name = this.state.name;
        }
        else if (this.state.type === "Coal") {
            powerPlant.maximumOutputPower = this.state.maximumOutputPower;
            powerPlant.minimumOutputPower = this.state.minimumOutputPower;
            powerPlant.type = "coal";
            powerPlant.name = this.state.name;
        }
        else if (this.state.type === "Gas") {
            powerPlant.maximumOutputPower = this.state.maximumOutputPower;
            powerPlant.minimumOutputPower = this.state.minimumOutputPower;
            powerPlant.type = "gas";
            powerPlant.name = this.state.name;
        }

        console.log(powerPlant);

        axios.post(baseUrl + "home/addpowerplant", powerPlant).then(response => {

            console.log("Dodavanje -> ", response.data);

            //poziv za dobavljanje novo dodatih generatora
            this.props.getAllGenerators();

            // ako prodje sve kako treba, zatvoriti modal
            this.props.toggleModal();

        }).catch(error => {
            if (error.response) {
                // Request made and server responded
                console.log(error.response.data);
                alert(error.response.data.Message);

            } else if (error.request) {
                // The request was made but no response was received
                console.log(error.request);
            }
        })


    }

    CloseModal = event => {
        //da ne zatvori modal kad se klikne na input polje
        if (event.target.id !== "overlay")
            return;

        this.setState({
            type: "wind",
            maximumOutputPower: 0,
            minimumOutputPower: 0,
            name: "Vetro",
            efficiency: 0,
            surfaceArea: 0,
            bladesSweptAreaDiameter: 10,
            numberOfWindGenerators: 10
        })
        this.props.toggleModal();
    }

    render() {
        return (
            <div className='modal-overlay' onClick={this.CloseModal} id="overlay">
                <div className="modal-form">
                    <h3 className='modal-header'>Generator form</h3>

                    <label className='modal-label' htmlFor="minProductionValue">Power plant production type</label>
                    <select className='modal-select' id='type' value={this.state.productionType} onChange={this.handleInputChange}>
                        {
                            powerPlantTypes.map(item => {
                                return <option key={item} value={item}>{item}</option>
                            })
                        }
                    </select>

                    <input className='modal-input' type="text" placeholder="Name" id="name" onChange={this.handleInputChange} />

                    {/* <label className='modal-label' htmlFor="maxProductionValue">{this.state.firstPlaceHolder}</label> */}
                    <input className='modal-input' min={1} type="number" placeholder={this.state.firstPlaceHolder} id="maximumOutputPower" onChange={this.handleInputChange} />

                    {/* <label className='modal-label' htmlFor="minProductionValue">{this.state.secondPlaceHolder} </label> */}
                    {this.state.minPowerDisabled ? null :
                        (< input className='modal-input' min={0} type="number" placeholder={this.state.secondPlaceHolder} id="minimumOutputPower" onChange={this.handleInputChange} />)
                    }





                    <button className='modal-submit' onClick={this.handleSubmit}>Submit</button>
                </div>
            </div>
        );
    }
}

export default Modal;