import React, { Component } from 'react';
import imgUpload from '../assets/fileUpload.png'
import imgCancel from '../assets/cancel.png'
import iconExcel from '../assets/excel.png'
import iconDocument from '../assets/anyDocument.png'

class FileUpload extends Component {
    constructor() {
        super();
        this.inputFileRef = React.createRef();
        this.imagesExtension = ["jpg", "jpeg", "png"]
        this.state = {
            fileUrl: null,
            fileName: "",
            file: null,
        }
    }


    handleCustomBtnClick = () => {

        this.inputFileRef.current.click();
    }

    handleFileChange = event => {
        console.log(event.target.files)
        var file = event.target.files[0];
        if (!file)
            return

        var url = URL.createObjectURL(file)
        var re = /(?:\.([^.]+))?$/;
        var extenstion = re.exec(file.name);
        console.log(extenstion)
        if (extenstion[1] === "xlsx") {
            this.setState({
                fileUrl: iconExcel,
                file: file,
                fileName: file.name,
            })
        }
        else if (this.imagesExtension.includes(extenstion[1])) {
            this.setState({
                fileUrl: url,
                file: file,
                fileName: file.name,
            })
        }
        else {
            this.setState({
                fileUrl: iconDocument,
                file: file,
                fileName: file.name,
            })
        }

    }

    handleOnCancelClick = () => {
        this.setState({
            fileUrl: null,
            fileName: "",
            file: null,
        })
    }

    render() {
        return (
            <div className="file-upload-body">
                <div className="container">
                    <div className={this.state.fileUrl === null ? "warpper" : "warpper active"}>
                        <div className="image">
                            <img src={this.state.fileUrl} alt="" />
                        </div>

                        <div className="content">
                            <div className="icon">
                                <img src={imgUpload} alt="" />
                            </div>
                            <div className="text">
                                No file chosen, yet!
                            </div>
                        </div>

                        <div id="cancel-btn" onClick={this.handleOnCancelClick}><img src={imgCancel} alt="close" /></div>
                        <div className="file-name">{this.state.fileName}</div>

                    </div>

                    <input id="default-btn" type="file" hidden={true} ref={this.inputFileRef} onChange={this.handleFileChange} />
                    <button id="custom-btn" onClick={this.handleCustomBtnClick}>Choose a file</button>
                </div>
            </div>
        );
    }
}

export default FileUpload;