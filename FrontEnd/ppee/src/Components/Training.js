import React, { Component } from 'react';
import { baseUrl } from '../constants';
import axios from 'axios';

class Training extends Component {
    constructor() {
        super();
        this.state = {
            fromDate: null,
            toDate: null,
        }
    }

    handleTraining = () => {
        
        //backend model props start wiht uppercase character
        var body ={
            FromDate: this.state.fromDate,
            ToDate: this.state.toDate,
        }

        axios.post(baseUrl + "home/training", body).then(response => {

            console.log("Stiglo je od treninga-> ", response.data);

        }).catch(error => {
            console.log(error);
        })

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

    render() {
        return (
            <div className="training-container">
                <h2 className='training_header'>Train model</h2>
                <div className='training_content'>
                    <span>From date <input id="fromDate" type="date" onChange={this.onDateChange} /> </span>
                    <span>To date <input id="toDate" type="date" onChange={this.onDateChange} /> </span>
                </div>

                <div className="btn_container btn_train">
                    <button className='btn' onClick={this.handleTraining}>Training</button>
                </div>
            </div>
        );
    }
}

export default Training;