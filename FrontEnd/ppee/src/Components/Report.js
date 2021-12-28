import React, { Component } from 'react';
import { baseUrl } from '../constants';
import axios from 'axios';


class Report extends Component {

    constructor() {
        super();
        this.state = {
            fromDate: null,
            toDate: null,
            data: undefined,
            exportFromDate: null,
            exportToDate: null,
        }

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

    handleShowReport = () => {
        //backend model props start wiht uppercase character
        var body = {
            FromDate: this.state.fromDate,
            ToDate: this.state.toDate,
        }

        console.log(body);

        axios.post(baseUrl + "home/report", body).then(response => {

            console.log("Stiglo je od report-a --> ", response.data);

            this.setState({
                data : JSON.parse(response.data),
                exportFromDate: body.FromDate,
                exportToDate: body.ToDate,
            })

        }).catch(error => {
            console.log(error);
        })
    }

    handleExport = () =>{

        var body = {
            FromDate: this.state.exportFromDate,
            ToDate: this.state.exportToDate,
        }

        console.log(body);

        axios.post(baseUrl + "home/export", body).then(response => {

            console.log("Stiglo je od exporta-a --> ", response.data);

        }).catch(error => {
            console.log(error);
        })
    }

    render() {
        return (
            <div className="training-container">
                <h2 className='training_header'>Report</h2>
                <div className='training_content'>
                    <div className='training_span_and_date'>
                        <span>From date</span>
                        <input id="fromDate" type="date" onChange={this.onDateChange} />
                    </div>
                    <div className='training_span_and_date'>
                        <span>To date</span><input id="toDate" type="date" onChange={this.onDateChange} />
                    </div>
                </div>

                <div className="btn_container btn_report">
                    <button className='btn' onClick={this.handleShowReport}>Show forecast</button>
                </div>

                {
                    this.state.data === undefined ? null : (
                        <div className='export_content'> 
                            <button className='btnExport' onClick={this.handleExport}> Export to CSV </button>
                        </div>
                    )
                }
                { this.state.data === undefined ? null :
                    (<div className="policy-container">
                        <div className="policy-table-report">
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

export default Report;