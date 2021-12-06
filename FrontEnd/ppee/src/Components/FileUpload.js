import React, { Component } from 'react';

class FileUpload extends Component {
    render() {
        return (
            <div>

                <div className="file-container">
                    <h3 className="file-heading">File Upload </h3>
                    <input type="file"></input>
                </div>
            </div>
        );
    }
}

export default FileUpload;