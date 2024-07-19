
function back() {
    window.history.back();
}

function showAlert(message) {
    window.alert(message);
}

function pies(jsData, chartId) {
    id = chartId
    //console.log(jsData);
    //console.log(jsData.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'));
    data = JSON.parse(jsData.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'))

    function explodePie(e) {
        if (typeof (e.dataSeries.dataPoints[e.dataPointIndex].exploded) === "undefined" || !e.dataSeries.dataPoints[e.dataPointIndex].exploded) {
            e.dataSeries.dataPoints[e.dataPointIndex].exploded = true;
        } else {
            e.dataSeries.dataPoints[e.dataPointIndex].exploded = false;
        }
        e.chart.render();
    }

    var chart = new CanvasJS.Chart(id, {
        theme: "dark",

        exportFileName: `${id} Chart`,
        exportEnabled: false,
        animationEnabled: true,
        title: {
            text: id.split(/(?=[A-Z])/).join(' '),
            fontSize: 20,
        },
        legend: {
            cursor: "pointer",
            itemclick: explodePie
        },
        data: [{
            type: "doughnut",
            startAngle: 60,
            innerRadius: 75,
            showInLegend: false,
            toolTipContent: "<b>{name}</b>: {percent}% ",
            indexLabel: "{name}: ({y}) ",
            dataPoints: data
        }]
    });

    try {

        chart.render();
    }
    catch (error) {
        console.log(error)
    }
}

function peak(data) {

    var dataPoints = [];

    data = JSON.parse(data.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'))
    //console.log(data)

    for (var i = 0; i < data.length; i++) {
        dataPoints.push({
            x: new Date(data[i].x),
            y: data[i].y
        });
    }

    var chart = new CanvasJS.Chart("PeakHour", {
        animationEnabled: true,
        theme: "light2",
        title: {
            text: "Peak Hour Sale"
        },
        axisY: {
            //title: "Total Amount",
            titleFontSize: 20,
            suffix: "K",
            labelFormatter: function (e) {
                return e.value / 1000;
            },
            crosshair: {
                enabled: true,
                valueFormatString: "#,##0.##",
                snapToDataPoint: true
            }
        },
        axisX: {
            crosshair: {
                enabled: true,
                snapToDataPoint: true
            }
        },
        data: [{
            type: "splineArea",
            yValueFormatString: "#,##0.##",
            xValueFormatString: "MMM DD YYYY",
            dataPoints: dataPoints
        }]

    })

    chart.render();
}
function peakDate(data) {
    console.log("Peakdate")
    console.log(data)
    var dataPoints = [];

    data = JSON.parse(data.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'))
    //console.log(data)

    for (var i = 0; i < data.length; i++) {
        dataPoints.push({
            x: new Date(data[i].x),
            y: data[i].y
        });
    }

    var chart = new CanvasJS.Chart("ByDay", {
        animationEnabled: true,
        theme: "light1",// "light1", "light2", "dark1", "dark2"
        title: {
            text: "Monthly Sales"
        },
        axisY: {
            //title: "Total Amount",
            titleFontSize: 20,
            suffix: "K",
            labelFormatter: function (e) {
                return e.value / 1000;
            },
            crosshair: {
                enabled: true,
                valueFormatString: "#,##0.##",
                snapToDataPoint: true
            }
        },
        axisX: {
            crosshair: {
                enabled: true,
                snapToDataPoint: true
            }
        },
        data: [{
            type: "splineArea",
            yValueFormatString: "#,##0.##",
            xValueFormatString: "MMM DD YYYY",
            dataPoints: dataPoints
        }]

    })

    chart.render();
}
function peakMonth(data) {

    var dataPoints = [];

    data = JSON.parse(data.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'))
    //console.log(data)

    for (var i = 0; i < data.length; i++) {
        dataPoints.push({
            x: new Date(data[i].x),
            y: data[i].y
        });
    }

    var chart = new CanvasJS.Chart("ByDay", {
        animationEnabled: true,
        theme: "light1",// "light1", "light2", "dark1", "dark2"
        title: {
            text: "Monthly Sales"
        },
        axisY: {
            //title: "Total Amount",
            titleFontSize: 20,
            suffix: "K",
            labelFormatter: function (e) {
                return e.value / 1000;
            },
            crosshair: {
                enabled: true,
                valueFormatString: "#,##0.##",
                snapToDataPoint: true
            }
        },
        axisX: {
            crosshair: {
                enabled: true,
                snapToDataPoint: true
            }
        },
        data: [{
            type: "splineArea",
            yValueFormatString: "#,##0.##",
            xValueFormatString: "MMM DD YYYY",
            dataPoints: dataPoints
        }]

    })

    chart.render();
}

function spline(data) {

    if (data !== null || data !== '') {
        let dataPoints = JSON.parse(data.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'))
        //console.log(dataPoints)
        let year = new Date().getFullYear();
        let total = dataPoints[0].total;
        var chart = new CanvasJS.Chart("Reservation", {
            theme: "light2",
            animationEnabled: true,
            title: {
                text: `Reservation(${year}) Total=${total}`
            },
            axisY: {

                crosshair: {
                    enabled: true,

                    snapToDataPoint: true
                }
            },
            axisX: {
                crosshair: {
                    enabled: true,
                    snapToDataPoint: true
                }
            },
            legend: {
                cursor: "pointer"
            },
            data: [{
                type: "splineArea",
                visible: true,
                showInLegend: false,
                yValueFormatString: "##,000",
                name: "Reservations",
                dataPoints: dataPoints
            }]

        });
        chart.render();
    }


    function toggleDataSeries(e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
            e.dataSeries.visible = false;
        } else {
            e.dataSeries.visible = true;
        }
        chart.render();
    }
}

function splineAreal(data, id, graphTitle) {
    console.log(data); // Corrected typo: console.log instead of cosole.log
    console.log(id);
    console.log(graphTitle);

    if (data !== null || data !== '') {
        try {
             let dataPoints = JSON.parse(data.replace(/&quot;/g, '"').replaceAll('"{', '{').replaceAll('}"', '}'));
            let year = new Date().getFullYear();
            var chart = new CanvasJS.Chart(id, {
                theme: "light2",
                animationEnabled: true,
                title: {
                    text: graphTitle
                },
                axisY: {
                    suffix: "M",
                    labelFormatter: function (e) {
                        return e.value / 1000000;
                    },
                    crosshair: {
                        enabled: true,
                        snapToDataPoint: true
                    }
                },
                axisX: {
                    interval: 1,
                    labelAngle: -90,
                    crosshair: {
                        enabled: true,
                        snapToDataPoint: true
                    }
                },
                legend: {
                    cursor: "pointer"
                },
                data: [{
                    type: "splineArea",
                    visible: true,
                    showInLegend: false,
                    yValueFormatString: "##,000",
                    name: "Reservations",
                    dataPoints: dataPoints
                }]
            });
            chart.render();
        } catch (error) {
            console.error("Error parsing JSON:", error);
        }
    }
}
