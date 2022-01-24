import React, { Component } from 'react';
import { baseUrl } from '../constants';
import axios from 'axios';

class Training extends Component {
    constructor() {
        super();
        this.state = {
            fromDate: null,
            toDate: null,
            messageFromServer: undefined
        }
    }

    handleTraining = () => {

        //backend model props start wiht uppercase character
        var body = {
            FromDate: this.state.fromDate,
            ToDate: this.state.toDate,
        }

        axios.post(baseUrl + "home/training", body).then(response => {

            console.log("Stiglo je od treninga-> ", response.data);
            this.setState({ messageFromServer: response.data })

        }).catch(error => {
            if (error.response) {
                // Request made and server responded
                console.log(error.response.data);
                alert(error.response.data);

            } else if (error.request) {
                // The request was made but no response was received
                console.log(error.request);
            }
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
                    <div className='training_span_and_date'>
                        <span>From date</span>
                        <input id="fromDate" type="date" onChange={this.onDateChange} />
                    </div>
                    <div className='training_span_and_date'>
                        <span>To date</span><input id="toDate" type="date" onChange={this.onDateChange} />
                    </div>
                </div>

                <div className="btn_container btn_train">
                    <button className='btn' onClick={this.handleTraining}>Training</button>
                </div>

                {
                    this.state.messageFromServer !== undefined ?
                        (<div className='training_responseMessage'>
                            <h3>{this.state.messageFromServer}</h3>
                        </div>) : null
                }
            </div>
        );
    }
}

export default Training;