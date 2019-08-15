// Write your JavaScript code.

function createSensorGraph(id, axisLabels, chartData, label) {
    let ctx = document.getElementById(id);
    let chart = new Chart(ctx, {
        type: 'line', data: {
            labels: axisLabels,
            datasets: [{
                label: label,
                fill: false,
                borderColor: "rgba(75,192,192,1)",
                backgroundColor: "rgba(75,192,192,0.4)",
                data: chartData
            }]
        },
        options: {
            scales: {
                yAxes: [{ ticks: { beginAtZero: true } }]
            },
            legend: { display: false }
        }
    });
}