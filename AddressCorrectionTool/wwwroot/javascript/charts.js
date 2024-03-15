var metricsChart;

async function createMetricsChart(data) {

    // convert data array to object
    console.log(data[0]);

    Chart.register(ChartDataLabels);

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
                            data[2]
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
                            }

                        },
                        borderWidth: 1
                    }
                ],
                labels: ['Corrected Addresses', 'Failed Addresses']
            },
            options: {
                responsive: true,
                title: {
                    display: true,
                    text: 'Metrics'
                },
            }
        }
    );
}

