function initializeChart(chartSection, id, label, color, min, max, nominalMin, nominalMax) {
    const chartContainer = document.createElement('div');
    chartContainer.classList.add('chart-container');
    chartContainer.classList.add('w-1/5');
    chartContainer.classList.add('border');
    const header = document.createElement('h1');
    header.id = id;
    header.style = "font-size:20px";
    const canvas = document.createElement('canvas');
    canvas.id = id;
    chartContainer.appendChild(header);
    chartContainer.appendChild(canvas);
    console.log(chartSection)

    document.getElementById(chartSection+'-charts-wrapper').appendChild(chartContainer);

    const ctx = canvas.getContext('2d');

    // Prepare annotations
    const annotations = {};
    // Add nominal range lines if nominalMin and nominalMax are provided
    if (nominalMin) {
        annotations.nominalRange = {
            type: 'line',
            yMin: nominalMin,
            yMax: nominalMin,
            borderColor: 'rgba(255, 0, 0, 0.5)', // Red color for the line
            borderWidth: 2,
            label: {
                content: 'Nominal Min',
                enabled: true,
                position: 'start'
            },
            borderDash: [5, 5] // Create a dashed line
        };

    }

    if (nominalMax) {
        console.log('test')
        annotations.nominalMaxRange = {
            type: 'line',
            yMin: nominalMax,
            yMax: nominalMax,
            borderColor: 'rgba(255, 0, 0, 0.5)', // Red color for the line
            borderWidth: 2,
            label: {
                content: 'Nominal Max',
                enabled: true,
                position: 'start'
            },
            borderDash: [5, 5] // Create a dashed line
        };

    }

    return new Chart(ctx, {
        type: 'line',
        data: {
            labels: [], // X-axis labels
            datasets: [{
                label: label,
                data: [], // This will be populated later
                backgroundColor: color,
                borderColor: color,
                fill: false,
                borderWidth: 1,
                pointBackgroundColor: function (context) {
                    const value = context.raw;
                    return value < nominalMin || value > nominalMax ? 'red' : color;
                }            }]
        },
        options: {
            scales: {
                y: {
                    min: min,
                    max: max
                }
            },
            plugins: {
                annotation: {
                    annotations: annotations
                }
            }
        }
    });
}