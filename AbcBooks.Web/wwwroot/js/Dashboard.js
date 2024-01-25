function yearGraph() {

    var element = document.getElementById("yd");
    element.classList.add("bg-primary");
    element.classList.add("text-light");

    var elementMd = document.getElementById("md");
    elementMd.classList.remove("bg-primary");
    elementMd.classList.remove("text-light");

    var monthlyLineElement = document.getElementById("monthlyLine");
    var yearlyLineElement = document.getElementById("yearlyLine");
    monthlyLineElement.style.display = "none";
    yearlyLineElement.style.display = "block";
}

function monthGraph() {
    var element = document.getElementById("md");
    element.classList.add("bg-primary");
    element.classList.add("text-light");

    var elementYd = document.getElementById("yd");
    elementYd.classList.remove("bg-primary");
    elementYd.classList.remove("text-light");

    var monthlyLineElement = document.getElementById("monthlyLine");
    var yearlyLineElement = document.getElementById("yearlyLine");
    monthlyLineElement.style.display = "block";
    yearlyLineElement.style.display = "none";
}

fetch('https://abcbooksweb.azurewebsites.net/Admin/YearlySales')
    .then(response => response.json())
    .then(jsonData => {
        const labels = jsonData.data.map(item => item.year);
        const dataPoints = jsonData.data.map(item => item.revenue);

        const ytx = document.getElementById('yearlyLine');

        new Chart(ytx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Yearly Revenue (INR)',
                    data: dataPoints,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        ytx.style.display = 'none';
    })
    .catch(error => console.error('Fetch error:', error));

fetch('https://abcbooksweb.azurewebsites.net/Admin/MonthlySales')
    .then(response => response.json())
    .then(jsonData => {
        const labels = jsonData.data.map(item => item.month);
        const dataPoints = jsonData.data.map(item => item.revenue);

        const ctx = document.getElementById('monthlyLine');

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Monthly Revenue (INR)',
                    data: dataPoints,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    })
    .catch(error => console.error('Fetch error:', error));

fetch('https://abcbooksweb.azurewebsites.net/Admin/RevenueByCategory')
    .then(response => response.json())
    .then(jsonData => {
        const labels = jsonData.data.map(item => item.category);
        const dataPoints = jsonData.data.map(item => item.revenue);

        const ztx = document.getElementById('doughnutChart');

        new Chart(ztx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: dataPoints,
                    borderWidth: 1
                }]
            },
        });
    })
    .catch(error => console.error('Fetch error:', error));

fetch(`https://abcbooksweb.azurewebsites.net/Admin/DailySales?fromDate=${new Date().toISOString() }&toDate${new Date().toISOString()}`)
    .then(response => response.json())
    .then(jsonData => {
        const labels = jsonData.data.map(item => item.date);
        const dataPoints = jsonData.data.map(item => item.revenue);

        const ztx = document.getElementById('dailyChart');

        new Chart(ztx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Daily Revenue (INR)',
                    data: dataPoints,
                    borderWidth: 1
                }]
            },
        });
    })
    .catch(error => console.error('Fetch error:', error));