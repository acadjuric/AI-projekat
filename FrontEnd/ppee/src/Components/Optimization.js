import React, { Component } from 'react';
import Modal from './Modal';
import delteTrash from '../assets/delete1.png'
import saveIcon from '../assets/save-icon.png'

const generators = [
    { maxValue: 30, minValue: 10, numberForOptimization: 2, type: "coal" },
    { maxValue: 20, minValue: 16, numberForOptimization: 2, type: "wind" },
    { maxValue: 50, minValue: 33, numberForOptimization: 2, type: "solar" },
    { maxValue: 47, minValue: 26, numberForOptimization: 2, type: "gas" },
    { maxValue: 56, minValue: 8, numberForOptimization: 2, type: "hydro" },
    
]

class Optimization extends Component {
    constructor() {
        super();
        this.state = {
            showModal: false,
            myGenerators: generators,
        }
    }

    toggleModal = () => {

        this.setState({ showModal: !this.state.showModal })
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

                <button className='button-43' onClick={this.toggleModal}>Add generator</button>
                {
                    this.state.showModal && (<Modal toggleModal={this.toggleModal} />)
                }

                <div className='card-container'>
                    {
                        this.state.myGenerators && this.state.myGenerators.map((item, index) => {
                            return (
                                <div className="card" key={index}>
                                    <div className={"card-image card-image-" + item.type}></div>
                                    <div className="card-text">
                                        <h3>Max value</h3>
                                        <p>{item.maxValue}</p>

                                        <h3>Min value</h3>
                                        <p>{item.minValue}</p>
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
        );
    }
}

export default Optimization;