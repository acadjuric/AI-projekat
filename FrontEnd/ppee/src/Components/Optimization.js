import React, { Component } from 'react';
import Modal from './Modal';

class Optimization extends Component {
    constructor(){
        super();
        this.state ={
            showModal:false,
        }
    }

    toggleModal =() =>{

        this.setState({ showModal: !this.state.showModal})
    }

    render() {
        return (
            <div>
                
                <button className='button-43' onClick={this.toggleModal}>Add generator</button>
                {
                    this.state.showModal && ( <Modal toggleModal={this.toggleModal} /> )
                }

            </div>
        );
    }
}

export default Optimization;