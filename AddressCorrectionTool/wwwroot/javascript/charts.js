var metricsChart;

Chart.register(ChartDataLabels);

async function createMetricsChart(data) {

    //register labels
    // Chart.register(ChartDataLabels);

    if (metricsChart) {
        metricsChart.destroy();
    }

    var ctx = document.getElementById('metrics-chart').getContext('2d');

    metricsChart = new Chart(
        ctx,
        {
            type: 'doughnut',
            data: {
                datasets: [
                    {
                        data: [
                            data[1],
                            data[2],
                            data[3]
                        ],
                        backgroundColor: [
                            // blue colour palette
                            '#009ABC',
                            '#8e5ea2',
                            '#3cba9f'

                        ],
                        datalabels: {
                            color: '#ffff',
                            font: {
                                size: '20',
                                weight: 'bold'
                            },
                            formatter: function (value, context) {
                                return value === 0 ? null : value;
                            }

                        },
                        borderWidth: 1
                    }
                ],
                labels: ['Corrected Addresses', 'Failed Addresses', 'Miscorrected Addresses']
            },
            options: {
                responsive: true,
                title: {
                    display: true,
                    text: 'Metrics'
                },
                plugins: {
                    legend: {
                        display: true,
                        labels: {
                            color: 'white'
                        }
                    }
                }
            }
        }
    );
}

var timeSeriesChart;

function createTimeSeriesChart(timeSeriesData) {

    const ctx = document.getElementById('timeseriesMetricsChart').getContext('2d');
    if (timeSeriesChart) {
        timeSeriesChart.destroy();
    }


    timeSeriesChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: timeSeriesData.map(item => new Date(item.timeStamp)),
            datasets: [{
                label: 'Processing Time',
                data: timeSeriesData.map(item => item.processingTime),
                borderColor: 'rgb(75, 192, 192)',
                borderCapStyle: 'round',
                tension: 0.3,
                datalabels: {
                    display: false
                }
            }]
        },
        options: {
            plugins: {
                zoom: {
                    pan: {
                        enabled: true,
                        mode: 'x'
                    },
                    zoom: {
                        wheel: {
                            enabled: true
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: 'x'
                    }
                },
                legend: {
                    labels: {
                        color: 'white'
                    }
                }
            },
            scales: {
                x: {
                    type: 'time',
                    title: {
                        display: true,
                        text: 'Time',
                        color: 'white'
                    },
                    time: {
                        unit: 'minute'
                    },
                    ticks: {
                        color: 'white' // change this to any valid CSS color
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Milliseconds',
                        color: 'white'
                    },
                    ticks: {
                        color: 'white' // change this to any valid CSS color
                    }
                }
            }
        }
    });
}