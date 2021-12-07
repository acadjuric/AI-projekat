import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import imgFile from '../assets/file.png'


class Navigation extends Component {
    constructor() {
        super();
        this.state = {
            tab_index: "1",
        }
    }

    componentDidMount = () => {

        console.log(window.location.pathname)
        if (window.location.pathname === "/file-upload")
            this.setState({ tab_index: "1" })
        else if (window.localStorage.pathname === "/training")
            this.setState({ tab_index: "2" })
        else if (window.localStorage.pathname === "/report")
            this.setState({ tab_index: "3" })
        else
            window.location.href="/file-upload"
            
        
            
    }

    handleOnClick = event => {
        
        if(event.target.name === undefined){
            var parent = event.target.parentElement
            console.log(parent)
            console.log(parent.name)
            this.setState({tab_index: parent.name})
        }
        else 
            this.setState({ tab_index: event.target.name })
    }

    render() {

        return (
            <div className="navigation">
                <ul>
                    <li className={this.state.tab_index === "1" ? "list active" : "list"} onClick={this.handleOnClick} name="1">
                        <Link className="aLink" to="/file-upload" name="1">
                            <span className="icon" name="1"><img src={imgFile} alt="img" name="1" /></span>
                            <span className={this.state.tab_index === "1" ? "text" : "hide"} name="1"> File upload </span>
                        </Link>
                    </li>

                    <li className={this.state.tab_index === "2" ? "list active" : "list"} onClick={this.handleOnClick} name="2">
                        <Link className="aLink" to="/training" name="2">
                            <span className="icon" name="2" ><img src={imgFile} alt="img" name="2" /></span>
                            <span className={this.state.tab_index === "2" ? "text" : "hide"} name="2" > Training </span>
                        </Link>
                    </li>

                    <li className={this.state.tab_index === "3" ? "list active" : "list"} onClick={this.handleOnClick} name="3">
                        <Link className="aLink" to="/report" name="3">
                            <span className="icon" name="3"><img src={imgFile} alt="img" name="3" /></span>
                            <span className="text" hidden={this.state.tab_index === "3"? false:true} name="3"> Report</span>
                        </Link>
                    </li>
                    <div className="indicator"></div>
                </ul>
            </div>
        );
    }
}

export default Navigation;