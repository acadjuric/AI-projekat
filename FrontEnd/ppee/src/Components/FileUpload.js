import React, { Component } from 'react';
import imgUpload from '../assets/fileUpload.png'
import imgCancel from '../assets/cancel.png'
import iconExcel from '../assets/excel.png'
import iconDocument from '../assets/anyDocument.png'
import { baseUrl } from '../constants';
import axios from 'axios';

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

    handleUploadBtnClick = () => {
        if (this.state.file === null || this.state.fileName === "") return;

        var form = new FormData();
        form.append("file", this.state.file, this.state.fileName);
        axios.post(baseUrl + "home/fileupload", form).then(response => {

            console.log("Stiglo je od servera-> ", response.data);

        }).catch(error => {
            console.log(error);
        })
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
                    <div className={this.state.fileUrl === null ? "warpper" : "warpper active"} >
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
                    <div className='wrapper_buttons'>
                        <div className="btn_container btn_fileUpload">
                            <button className='btn' onClick={this.handleCustomBtnClick}>Choose a file</button><br />
                        </div>
                        <div className="btn_container btn_sendFile">
                            <button className="btn" onClick={this.handleUploadBtnClick}>Send file</button>
                        </div>
                    </div>

                </div>
            </div>
        );
    }
}

export default FileUpload;