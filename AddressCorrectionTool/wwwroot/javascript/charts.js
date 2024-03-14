
async function createMetricsChart(data) {

    // convert data array to object
    console.log(data[0]);

    Chart.register(ChartDataLabels);

    new Chart(
        document.getElementById('metrics-chart'),
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
                            '#3e95cd',
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
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: 'Metrics'
                },
            }
        }
    );
}

