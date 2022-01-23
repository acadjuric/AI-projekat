import React, { Component } from 'react';
import Modal from './Modal';
import delteTrash from '../assets/delete1.png'
import saveIcon from '../assets/save-icon.png'
import { baseUrl } from '../constants';
import axios from 'axios';

const generators = [
    { maximumOutputPower: 30, minimumOutputPower: 10, numberForOptimization: 2, type: "coal" },
    { maximumOutputPower: 20, minimumOutputPower: 16, numberForOptimization: 2, type: "wind" },
    { maximumOutputPower: 50, minimumOutputPower: 33, numberForOptimization: 2, type: "solar" },
    { maximumOutputPower: 47, minimumOutputPower: 26, numberForOptimization: 2, type: "gas" },
    { maximumOutputPower: 56, minimumOutputPower: 8, numberForOptimization: 2, type: "hydro" },

]

class Optimization extends Component {
    constructor() {
        super();
        this.state = {
            showModal: false,
            myGenerators: generators,
            showSettings: false,
        }
    }

    componentDidMount = () => {

        this.GetAllGenerators();

    }

    GetAllGenerators = () => {
        axios.get(baseUrl + "home/getpowerplant").then(response => {

            console.log("Server odgovorio-> ", response);
            this.setState({myGenerators : response.data})

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

    deleteGenerator = (index) => {
        if (generators[index] === undefined)
            return;

        console.log(generators[index]);
    }

    SaveNumberOfGenerators = (index) => {

        if (this.state.myGenerators[index] === undefined)
            return;

        console.log(this.state.myGenerators[index]);
    }

    handleInputChange = (index, event) => {
        if (generators[index] === undefined)
            return;

        console.log(generators[index]);
        generators[index].numberForOptimization = event.target.value;
        console.log(generators[index]);
        this.setState({ myGenerators: generators })
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
                            <button className='button-43' onClick={this.toggleModal}>Add generator</button>
                            <h1> ovde ide izbor kirterijumske funkcije i podesavanje cene goriva kao i emisije CO2 </h1>
                        </div>

                        <div className='card-container'>
                            {
                                this.state.myGenerators && this.state.myGenerators.map((item, index) => {
                                    return (
                                        <div className="card" key={index}>
                                            <div className={"card-image card-image-" + item.type}></div>
                                            <div className="card-text">
                                                <h3>Max value</h3>
                                                <p>{item.maximumOutputPower}</p>

                                                <h3>Min value</h3>
                                                <p>{item.minimumOutputPower}</p>
                                                <h3>Optimization number</h3>
                                                <input value={item.numberForOptimization} onChange={(event) => this.handleInputChange(index, event)} />

                                                <div className='card-BtnContainer'>

                                                    <button className='BtnContainer-save' onClick={() => this.SaveNumberOfGenerators(index)}><img src={saveIcon} alt='save' /></button>

                                                    <button className='BtnContainer-delete' onClick={() => this.deleteGenerator(index)}><img src={delteTrash} alt='delete' /></button>

                                                </div>
                                            </div>
                                            <div className={"card-stats card-stats-" + item.type}>
                                                <div className="stat">
                                                    <div className="type">{item.type}</div>
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