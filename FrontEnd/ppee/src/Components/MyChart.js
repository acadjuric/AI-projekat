import React, { Component } from 'react';
import ReactApexChart from 'react-apexcharts';

class MyChart extends Component {
    constructor(props) {
        super(props);

        this.state = {

            series: [100, 150, 300, 500, 300],
            options: {
                colors:['#ecb22e', '#23b67d', '#36c5f0', '#4a154b', "#e01e5a"],
                labels:["Solar", "Wind", "Hydro", "Coal", "Gas"],
                chart: {
                    type: 'donut',
                },
                plotOptions: {
                    pie: {
                        startAngle: -90,
                        endAngle: 270,
                        donut: {
                            size: '60%'
                          }
                    }
                },
                dataLabels: {
                    enabled: false
                },
                fill: {
                    type: 'gradient',
                },
                legend: {
                    position: 'bottom',
                    horizontalAlign: 'center', 
                    fontSize: '16px',              
                    formatter: function (val, opts) {
                        return val + " - " + opts.w.globals.series[opts.seriesIndex] + " [MW]";
                    },
                    labels: {
                        colors: ['#ffffff','#ffffff','#ffffff','#ffffff','#ffffff'],
                        useSeriesColors: false
                    },
                    itemMargin: {
                        vertical: 1,
                    },
                },
                title: {
                    text: 'Distribution of production between types of generators',
                    style: {
                        fontSize:  '20px',
                        fontWeight:  'bold',
                        color:  '#fff',
                      },
                },
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: {
                            width: 200
                        },
                        legend: {
                            position: 'bottom'
                        }
                    }
                }]
            },


        };
    }

    render() {
        return (
            <div id="chart">
                <ReactApexChart colors={this.state.colors} options={this.state.options} series={this.state.series} type="donut" width={550} />
            </div>
        );
    }
}

export default MyChart;