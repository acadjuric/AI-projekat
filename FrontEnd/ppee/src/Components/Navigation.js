import React, { Component } from 'react';
import { Link } from 'react-router-dom';

class Navigation extends Component {

    render() {

        return (
            <div className="navigation">
                <Link to="/file-upload">File upload</Link>
                <Link to="/training">Training</Link>
                <Link to="/report">Report</Link>
            </div>
        );
    }
}

export default Navigation;