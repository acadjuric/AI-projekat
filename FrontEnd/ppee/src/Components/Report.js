import React, { Component } from 'react';

const days = [1, 2, 3, 4, 5, 6, 7]

class Report extends Component {

    constructor() {
        super();
        this.state = {
            fromDate: null,
            numberOfDays: '0'
        }

    }


    handleSelectChange = event =>{

        this.setState({[event.target.name] : event.target.value})
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
        console.log(this.state);
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
                
            </div>
        
        );
    }
}

export default Report;