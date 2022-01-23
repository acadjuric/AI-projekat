import React, { Component } from 'react';
import Modal from './Modal';
import delteTrash from '../assets/delete1.png'
// import saveIcon from '../assets/save-icon.png'
import { baseUrl } from '../constants';
import axios from 'axios';

const generators = [
    { maximumOutputPower: 30, minimumOutputPower: 10, numberForOptimization: 2, type: "coal" },
    { maximumOutputPower: 20, minimumOutputPower: 16, numberForOptimization: 2, type: "wind" },
    { maximumOutputPower: 50, minimumOutputPower: 33, numberForOptimization: 2, type: "solar" },
    { maximumOutputPower: 47, minimumOutputPower: 26, numberForOptimization: 2, type: "gas" },
    { maximumOutputPower: 56, minimumOutputPower: 8, numberForOptimization: 2, type: "hydro" },

]

const generatorsAndTheirNumberForOptimization = [];

class Optimization extends Component {
    constructor() {
        super();
        this.state = {
            showModal: false,
            myGenerators: generators,
            showSettings: true,
            isMouseInside: false,
        }
    }

    componentDidMount = () => {

        this.GetAllGenerators();

    }

    GetAllGenerators = () => {
        axios.get(baseUrl + "home/getpowerplant").then(response => {

            console.log("Server odgovorio-> ", response);
            this.setState({ myGenerators: JSON.parse(response.data) })

        }).catch(error => {
            console.log(error);
        })
    }
    toggleModal = () => {

        this.setState({ showModal: !this.state.showModal });
    }

    toggleSettings = () => {
        this.setState({ showSettings: !this.state.showSettings });
    }

    deleteGenerator = (id) => {
        console.log(id);
        axios.delete(baseUrl + "home/deletepowerplant/" + id).then(response => {
            console.log(response);
            this.getAllGenerators();
        }).catch(error => {
            console.log(error);
        })
    }

    handleMouseEnter = () => {
        this.setState({ isMouseInside: true })
    }

    handleMouseLeave = () => {
        this.setState({ isMouseInside: false })
    }
    SaveNumberOfGenerators = (index) => {

        // if (this.state.myGenerators[index] === undefined)
        //     return;

        // console.log(this.state.myGenerators[index]);
    }

    handleInputChange = (id, event) => {
        console.log(id);
        var modelExist = generatorsAndTheirNumberForOptimization.find(x => x.id === id);
        console.log(modelExist)
        if (modelExist === undefined) {
            var model = {
                id: id,
                number: event.target.value,
            }

            generatorsAndTheirNumberForOptimization.push(model)
        }
        else {
            var index = generatorsAndTheirNumberForOptimization.indexOf(modelExist);
            modelExist.number = event.target.value
            generatorsAndTheirNumberForOptimization[index] = modelExist
        }

        console.log(generatorsAndTheirNumberForOptimization);


        // console.log(generators[index]);
        // generators[index].numberForOptimization = event.target.value;
        // console.log(generators[index]);
        // this.setState({ myGenerators: generators })
    }

    render() {
        return (
            <div className='optimization-container'>

                <div className='optimization-options'>
                    <button className='button-43' onClick={this.toggleModal}>Add generator</button>
                    <button className='btn-showHide-settings' onClick={this.toggleSettings}>{this.state.showSettings === true ? "Hide Settings" : "Show Settings"}</button>

                </div>

                {
                    this.state.showModal && (<Modal toggleModal={this.toggleModal} getAllGenerators={this.GetAllGenerators} />)
                }
                {
                    this.state.showSettings &&

                    <div className='settings-container'>

                        <div className='basic-settings'>
                            {/* <button className='button-43' onClick={this.toggleModal}>Add generator</button> */}

                            <div className='settings-column'>
                                <div className='settings-box'>
                                    <label>Coal fuel cost</label> <input type="number" />
                                </div>

                                <div className='settings-box'>
                                    <label>Gas fuel cost</label> <input type="number" />
                                </div>
                            </div>
                            <div className='settings-column'>
                                <div className='settings-box'>
                                    <label>Coal CO2 emission</label> <input type="number" />
                                </div >

                                <div className='settings-box'>
                                    <label>Gas CO2 emission</label> <input type="number" />
                                </div>
                            </div>
                            <div className='settings-column'>
                                <div className='settings-box'>
                                    <select className='settings-select'>
                                        <option value="cost">Minimize production costs</option>
                                        <option value="co2">Minimize CO2 emissions</option>
                                        <option value="both">Fuel cost and CO2</option>
                                    </select>
                                </div>

                                <div className='settings-box'>
                                    <label>Fuel cost weight factor</label> <input type="number" />
                                </div>
                            </div>

                        </div>

                        <div className='card-container'>
                            {
                                this.state.myGenerators && this.state.myGenerators.map((item, index) => {
                                    return (
                                        <div className="card" key={index} onMouseEnter={this.handleMouseEnter} onMouseLeave={this.handleMouseLeave}>
                                            <div className={"card-image card-image-" + item.Type}></div>
                                            <div className="card-text">

                                                <div className="name-text">

                                                    <h3>Name</h3>
                                                    <p>{item.Name}</p>
                                                </div>

                                                <div className="max-text">
                                                    <h3>Max value</h3>
                                                    <p>{item.MaximumOutputPower} {item.Type === "solar" ? "[W]" : item.Type === "wind" ? "[KW]" : "[MW]"}</p>
                                                </div>

                                                {
                                                    item.Type === "coal" || item.Type === "gas" ? (<div className="min-text"> <h3>Min value</h3> <p>{item.MinimumOutputPower} [MW] </p></div>) : null
                                                }
                                                {/* {item.Type === "coal" || item.Type === "gas" ? (<h3>Min value</h3>) : null}
                                                {item.Type === "coal" || item.Type === "gas" ? (<p>{item.MinimumOutputPower} [MW] </p>) : null} */}


                                                {
                                                    item.Type === "solar" ? (<div className="min-text"> <h3>Efficiency</h3> <p>{item.Efficiency} [%] </p></div>) : null
                                                }

                                                {
                                                    item.Type === "wind" ? (<div className="min-text"> <h3>Diameter</h3> <p>{item.BladesSweptAreaDiameter} [m] </p></div>) : null
                                                }

                                                <div className="number-text" >
                                                    <h3>Optimization number</h3>
                                                    <input onChange={(event) => this.handleInputChange(item.Id, event)} />
                                                </div>


                                            </div>
                                            <div className='card-BtnContainer'>

                                                {/* <button className='BtnContainer-save' onClick={() => this.SaveNumberOfGenerators(item.Id)}><img src={saveIcon} alt='save' /></button> */}
                                                {
                                                    this.state.isMouseInside ? <button className='BtnContainer-delete' onClick={() => this.deleteGenerator(item.Id)}><img src={delteTrash} alt='delete' /></button> : null
                                                }


                                            </div>
                                            <div className={"card-stats card-stats-" + item.Type}>
                                                <div className="stat">
                                                    <div className="type">{item.Type}</div>
                                                </div>
                                            </div>
                                        </div>

                                    )
                                })
                            }

                        </div>
                    </div>
                }
            </div>
        );
    }
}

export default Optimization;