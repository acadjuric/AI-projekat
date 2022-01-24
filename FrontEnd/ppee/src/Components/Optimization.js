import React, { Component } from 'react';
import Modal from './Modal';
import delteTrash from '../assets/delete1.png'
// import saveIcon from '../assets/save-icon.png'
import { baseUrl } from '../constants';
import axios from 'axios';
import MyChart from './MyChart';

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
        this.child = React.createRef();

        this.state = {
            chartData: [],
            optimizationResult: [],
            showModal: false,
            myGenerators: generators,
            showSettings: false,
            isMouseInside: false,
            date: "",
            optimizationType: "",
            costCoal: 0,
            costGas: 0,
            cO2Gas: 0,
            cO2Coal: 0,
            weightFactor: 0,
            powerPlantAndNumberForOptimization: []
        }
    }

    componentDidMount = () => {

        this.GetAllGenerators();
        this.GetDefaultSettings();
    }

    GetAllGenerators = () => {
        axios.get(baseUrl + "home/getpowerplant").then(response => {

            console.log("Server odgovorio-> ", response);
            this.setState({ myGenerators: JSON.parse(response.data) })

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

    GetDefaultSettings = () => {

        axios.get(baseUrl + "home/optimizationsettings").then(response => {

            console.log("Server odgovorio-> ", JSON.parse(response.data));
            response.data = JSON.parse(response.data)
            this.setState({
                cO2Coal: response.data.CO2Coal,
                cO2Gas: response.data.CO2Gas,
                costGas: response.data.CostGas,
                costCoal: response.data.CostCoal,
                optimizationType: response.data.OptimizationType,
                weightFactor: response.data.WeightFactor,
                powerPlantAndNumberForOptimization: response.data.PowerPlantAndNumberForOptimization
            })

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

    handleMouseEnter = () => {
        this.setState({ isMouseInside: true })
    }

    handleMouseLeave = () => {
        this.setState({ isMouseInside: false })
    }

    handleInputChange = (id, event) => {
        console.log(id);
        var modelExist = generatorsAndTheirNumberForOptimization.find(x => x.id === id);
        console.log(modelExist)
        if (modelExist === undefined) {
            var model = {
                id: id,
                number: parseInt(event.target.value.toString()),
            }

            generatorsAndTheirNumberForOptimization.push(model)
        }
        else {
            var index = generatorsAndTheirNumberForOptimization.indexOf(modelExist);
            modelExist.number = parseInt(event.target.value)
            generatorsAndTheirNumberForOptimization[index] = modelExist
        }

        this.setState({
            powerPlantAndNumberForOptimization: generatorsAndTheirNumberForOptimization
        })

    }

    onDateChange = (event) => {

        var date = new Date(event.target.value)
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var day = date.getDate();

        if (day < 10) day = "0" + day;
        if (month < 10) month = "0" + month;


        var validFormatDate = day + "/" + month + "/" + year;
        this.setState({ [event.target.id]: validFormatDate })
        console.log(this.state);
    }

    handleSettingsInputChange = (event) => {
        this.setState({ [event.target.id]: event.target.value })
    }

    getValidDateFormatForOutput = (date, load) => {
        // input date in format -> yyyy-MM-dd(T)HH:mm:ss; Example -> 2019-01-04T13:00:00
        var parts = date.split('T');
        var dateParts = parts[0].split('-');
        var timeParts = parts[1].split(':');

        //output -> MM/dd/yyyy  HH:mm   --> load
        var dateForOutput = dateParts[1] + "/" + dateParts[2] + "/" + dateParts[0] + "  " + timeParts[0] + ":" + timeParts[1] + "  =>  " + load;
        return dateForOutput;
    }

    handleOptimize = () => {
        // console.log(this.state.powerPlantAndNumberForOptimization.filter(x=> isNaN(x.number) === false))
        // console.log(this.state)

        var body = {
            date: this.state.date,
            optimizationType: this.state.optimizationType,
            costCoal: this.state.costCoal,
            costGas: this.state.costGas,
            cO2Gas: this.state.cO2Gas,
            cO2Coal: this.state.cO2Coal,
            weightFactor: this.state.weightFactor,
            powerPlantAndNumberForOptimization: this.state.powerPlantAndNumberForOptimization.filter(x => isNaN(x.number) === false)
        }

        console.log(body);

        axios.post(baseUrl + "home/optimize", body).then(response => {

            try {
                console.log("Server->", JSON.parse(response.data));

                response.data = JSON.parse(response.data);
                var solarLoad = 0;
                var windLoad = 0;
                var hydroLoad = 0;
                var coalLoad = 0;
                var gasLoad = 0;

                for (let item of response.data) {

                    solarLoad += item.SolarLoad;
                    windLoad += item.WindLoad;
                    hydroLoad += item.HydroLoad;
                    gasLoad += item.GasLoad;
                    coalLoad += item.CoalLoad;

                }
                solarLoad = Math.round(solarLoad);
                windLoad = Math.round(windLoad);
                hydroLoad = Math.round(hydroLoad);
                gasLoad = Math.round(gasLoad);
                coalLoad = Math.round(coalLoad);

                var dataForChart = [solarLoad, windLoad, hydroLoad, coalLoad, gasLoad];

                if (this.state.chartData.length > 0) {
                    this.child.current.setNewDataFromParent(dataForChart);
                }


                this.setState({
                    optimizationResult: response.data,
                    chartData: dataForChart,
                });

            }
            catch (e) {
                // alert(e)
                alert(response.data);
            }

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

    render() {
        return (
            <div className='optimization-container'>

                <div className='optimization-options'>
                    <button className='button-43' onClick={this.toggleModal}>Add generator</button>
                    <button className='btn-showHide-settings' onClick={this.toggleSettings}>{this.state.showSettings === true ? "Hide Settings" : "Show Settings"}</button>

                </div>
                <div className='optimization-box'>
                    <span> For date <input id="date" type="date" onChange={this.onDateChange} /> </span>
                    <button className='btn-44' onClick={this.handleOptimize}>Optimize</button>
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
                                    <label>Coal fuel cost</label> <input type="number" id="costCoal" onChange={this.handleSettingsInputChange} value={this.state.costCoal} />
                                </div>

                                <div className='settings-box'>
                                    <label>Gas fuel cost</label> <input type="number" id="costGas" onChange={this.handleSettingsInputChange} value={this.state.costGas} />
                                </div>
                            </div>
                            <div className='settings-column'>
                                <div className='settings-box'>
                                    <label>Coal CO2 emission</label> <input type="number" id="cO2Coal" onChange={this.handleSettingsInputChange} value={this.state.cO2Coal} />
                                </div >

                                <div className='settings-box'>
                                    <label>Gas CO2 emission</label> <input type="number" id="cO2Gas" onChange={this.handleSettingsInputChange} value={this.state.cO2Gas} />
                                </div>
                            </div>
                            <div className='settings-column'>
                                <div className='settings-box'>
                                    <select className='settings-select' id="optimizationType" onChange={this.handleSettingsInputChange}>
                                        <option value="cost">Minimize production costs</option>
                                        <option value="co2">Minimize CO2 emissions</option>
                                        <option value="both">Production cost and CO2</option>
                                    </select>
                                </div>

                                {this.state.optimizationType === "both" ? (
                                    <div className='settings-box'>
                                        <label>Fuel cost weight factor</label> <input type="number" value={this.state.weightFactor} min={0} max={1} id="weightFactor" onChange={this.handleSettingsInputChange} />
                                    </div>
                                ) : null}
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
                                                    <input type="number" min={0} onChange={(event) => this.handleInputChange(item.Id, event)} />
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

                {
                    this.state.chartData && this.state.chartData.length > 0 ? (
                        <div className='optimization-chart'>
                            <MyChart ref={this.child} data={this.state.chartData} />
                        </div>
                    ) : null
                }


                <div className='tables-container'>
                    {
                        this.state.optimizationResult && this.state.optimizationResult.map((optimizationPerHour, index) => {
                            return (
                                <div className='one-table' key={index}>
                                    <div className="tbl-header">
                                        <table className='optimization-table' cellPadding="0" cellSpacing="0" border="0">
                                            <thead>
                                                <tr>
                                                    <th colSpan={5} className="opt-th th-datetime">{this.getValidDateFormatForOutput(optimizationPerHour.DateTimeOfOptimization, optimizationPerHour.Load)}</th>
                                                </tr>
                                                <tr>
                                                    <th className="opt-th">Name</th>
                                                    <th className="opt-th">Type</th>
                                                    <th className="opt-th">Load</th>
                                                    <th className="opt-th">Cost</th>
                                                    <th className="opt-th">CO2</th>
                                                </tr>
                                            </thead>
                                        </table>
                                    </div>
                                    <div className="tbl-content">
                                        <table className='optimization-table' cellPadding="0" cellSpacing="0" border="0">
                                            <tbody>
                                                {
                                                    optimizationPerHour.LoadsFromPowerPlants && optimizationPerHour.LoadsFromPowerPlants.map((optimizedData, index) => {
                                                        return (
                                                            <tr key={optimizationPerHour.DateTimeOfOptimization + index.toString()}>

                                                                <td className="opt-td">{optimizedData.Name}</td>
                                                                <td className="opt-td">{optimizedData.Type}</td>
                                                                <td className="opt-td">{optimizedData.Load} [MW]</td>
                                                                <td className="opt-td">{optimizedData.Cost} $</td>
                                                                <td className="opt-td">{optimizedData.CO2} [BTU]</td>

                                                            </tr>
                                                        )
                                                    })
                                                }

                                                <tr>
                                                    <td className='opt-td'>FINAL</td>
                                                    <td className='opt-td'></td>
                                                    <td className='opt-td'>{optimizationPerHour.LoadSum} [MW]</td>
                                                    <td className='opt-td'>{optimizationPerHour.Price} $</td>
                                                    <td className='opt-td'>{optimizationPerHour.Emission} [BTU]</td>
                                                </tr>
                                            </tbody>
                                        </table>
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