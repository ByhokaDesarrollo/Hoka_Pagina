

// TABLA DE ALMACEN PARA HOME/INICIO
$(document).ready(function () {
    // Create DataTable
    var table = $('#TablaHomeAlmacen').DataTable({
        lengthChange: false,
    });

    $('#cantidadAlmacen').text(table.page.info().recordsDisplay);

    // update the count on each draw
    table.on('draw', function () {
        $('#cantidadAlmacen').text(table.page.info().recordsDisplay);
    });

    // Create the chart with initial data
    var container = $('<div />').insertBefore(table.table().container());

    var chart = Highcharts.chart(container[0], {
        chart: {
            type: 'pie',
        },
        title: {
            text: 'Almacen Por Nombre',
        },
        series: [
            {
                data: chartData(table),
            },
        ],
    });

    // On each draw, update the data in the chart
    table.on('draw', function () {
        chart.series[0].setData(chartData(table));
    });
});

function chartData(table) {
    var counts = {};

    // Count the number of entries for each name
    table
        .column(1, { search: 'applied' })
        .data()
        .each(function (val) {
            if (counts[val]) {
                counts[val] += 1;
            } else {
                counts[val] = 1;
            }
        });

    // And map it to the format highcharts uses
    return $.map(counts, function (val, key) {
        return {
            name: key,
            y: val,
        };
    });
}

//TABLA DE PRODUCTOS PARA HOME/INICIO

$(document).ready(function () {
    // Create DataTable
    var table = $('#TablaHomeRemisiod').DataTable({
        lengthChange: false,
    });

    $('#cantidadRemisiod').text(table.page.info().recordsDisplay);

    // update the count on each draw
    table.on('draw', function () {
        $('#cantidadRemisiod').text(table.page.info().recordsDisplay);
    });

    // Create the chart with initial data
    var container = $('<div />').insertBefore(table.table().container());

    var chart = Highcharts.chart(container[0], {
        chart: {
            type: 'pie',
        },
        title: {
            text: 'Productos Por Almacenes',
        },
        series: [
            {
                data: chartData(table),
            },
        ],
    });

    // On each draw, update the data in the chart
    table.on('draw', function () {
        chart.series[0].setData(chartData(table));
    });
});

function chartData(table) {
    var counts = {};

    // Count the number of entries for each name
    table
        .column(1, { search: 'applied' })
        .data()
        .each(function (val) {
            if (counts[val]) {
                counts[val] += 1;
            } else {
                counts[val] = 1;
            }
        });

    // And map it to the format highcharts uses
    return $.map(counts, function (val, key) {
        return {
            name: key,
            y: val,
        };
    });
}
