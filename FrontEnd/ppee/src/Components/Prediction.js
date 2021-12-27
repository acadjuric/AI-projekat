import React, { Component } from 'react';
import { baseUrl } from '../constants';
import axios from 'axios';

const days = [1, 2, 3, 4, 5, 6, 7]

// const results = [
//     { id: 1, date: "15/12/2012", time: "13:00", MWh: 15000 },
//     { id: 2, date: "20/12/2012", time: "15:00", MWh: 16000 },
//     { id: 3, date: "25/12/2012", time: "17:00", MWh: 17000 },
//     { id: 4, date: "30/12/2012", time: "20:00", MWh: 18000 },
//     { id: 5, date: "15/12/2012", time: "13:00", MWh: 15000 },
//     { id: 6, date: "20/12/2012", time: "15:00", MWh: 16000 },
//     { id: 7, date: "25/12/2012", time: "17:00", MWh: 17000 },
//     { id: 8, date: "30/12/2012", time: "20:00", MWh: 18000 },
// ]

class Prediction extends Component {

    constructor() {
        super();
        this.state = {
            fromDate: null,
            numberOfDays: '0',
            data: undefined,
            mapeError: undefined,

        }

    }

    handleSelectChange = event => {

        this.setState({ [event.target.name]: event.target.value })
    }

    onDateChange = event => {

        var date = new Date(event.target.value)
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var day = date.getDate();

        var validFormatDate = day + "/" + month + "/" + year;
        this.setState({ [event.target.id]: validFormatDate })
        console.log(this.state);
    }

    handlePredict = () => {
        //backend model props start wiht uppercase character
        var body = {
            StartDate: this.state.fromDate,
            NumberOfDays: this.state.numberOfDays,
        }

        console.log(body);

        axios.post(baseUrl + "home/forecast", body).then(response => {

            console.log("Stiglo je od predicta-> ", response.data );
            
            this.setState({ 
                data: JSON.parse(response.data.m_Item1),
                mapeError: response.data.m_Item2,
            })

        }).catch(error => {
            console.log(error);
        })
    }

    render() {
        return (
            <div className="report-container">
                <h2 className='prediction_header'>Prediction</h2>
                <div className='prediction_content'>
                    <span>From date <input id="fromDate" type="date" onChange={this.onDateChange} /> </span>
                    <div className='select'>
                        <select name='numberOfDays' value={this.state.numberOfDays} onChange={this.handleSelectChange}>
                            <option value='0' disabled>Number of days</option>
                            {
                                days.map(item => {
                                    return <option key={item} value={item}>{item}</option>
                                })
                            }
                        </select>
                    </div>
                </div>

                <div className="btn_container btn_predict">
                    <button className='btn' onClick={this.handlePredict}>Predict</button>
                </div>

                {
                    this.state.mapeError === undefined ? null: <h4> Apsolutna greska - {this.state.mapeError}</h4>
                }

                { this.state.data === undefined ? null :
                    (<div className="policy-container">
                        <div className="policy-table">
                            <div className="headings">
                                <span className="heading">Date</span>
                                <span className="heading">Time</span>
                                <span className="heading">MWh</span>
                            </div>
                            {
                                this.state.data.map((item, index) => {
                                    return (
                                        <div className="policy" key={item.DateAndTime}>
                                            <span>{item.DateAndTime.split(" ")[0]}</span>
                                            <span>{item.DateAndTime.split(" ")[1]}</span>
                                            <span>{item.Load}</span>
                                        </div>
                                    )
                                })
                            }
                        </div>
                    </div>)
                }

            </div>

        );
    }
}

export default Prediction;