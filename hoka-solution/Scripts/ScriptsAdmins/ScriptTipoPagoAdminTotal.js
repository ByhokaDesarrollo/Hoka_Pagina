$(document).ready(function () {

    var $filtrarBusqueda = $('#filtrarBusqueda');
    var $tablaVTipoPago = $('#tablaVTipoPago');


    function isValidDate(d) {
        return d instanceof Date && !isNaN(d);
    }

    // modificaciones de los selects fecha
    $("#fechaInicio, #fechaFin").datepicker({
        format: 'dd/mm/yyyy',
        language: 'es',
        autoclose: true,
        todayHighlight: true
    });

    $("#fechaInicio, #fechaFin").on("keyup", function () {
        var valor = $(this).val().replace(/\D/g, "").replace(/^(\d\d)(\d)/g, "$1/$2").replace(/\/(\d\d)(\d)/, "/$1/$2");
        $(this).val(valor.substring(0, 10)); // Limitar a 10 caracteres
    });

    $("#fechaInicio, #fechaFin").keypress(function (e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        if ($(this).val().length >= 10 && (charCode >= 48 && charCode <= 57)) {
            return false;
        } else if (charCode >= 48 && charCode <= 57) {
            return true;
        }
        return false;
    });

    $("#fechaInicio, #fechaFin").on('paste', function (e) {
        e.preventDefault();
    });

    $('.select2').select2();

    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });


    function pivotData(data) {
        var pivotedData = {};
        var categories = new Set();
        var totalsByCategory = {}; // para almacenar los totales por categoría
        var totalGeneral = 0;

        data.forEach(row => {
            var moneda = row.moneda;
            var categoria = row.categoria;
            var total = parseFloat(row.total.toFixed(2)); // redondear a 3 decimales

            categories.add(categoria);

            if (!(moneda in pivotedData)) {
                pivotedData[moneda] = {};
            }

            pivotedData[moneda][categoria] = parseFloat(((pivotedData[moneda][categoria] || 0) + total).toFixed(3));

            totalsByCategory[categoria] = parseFloat(((totalsByCategory[categoria] || 0) + total).toFixed(3));
            totalGeneral = parseFloat((totalGeneral + total).toFixed(3));
        });

        // Agregar columna "Total" por cada fila
        for (let moneda in pivotedData) {
            pivotedData[moneda]["Total"] = Object.values(pivotedData[moneda]).reduce((a, b) => a + b, 0);
        }

        // Preparar la fila "Total"
        var totalRow = { "Moneda": "Total" };
        for (let categoria of categories) {
            totalRow[categoria] = totalsByCategory[categoria] || 0;
        }
        totalRow["Total"] = totalGeneral;

        // Convertir el objeto de datos pivoteados a una matriz para usar con DataTables
        var result = [];
        for (let moneda in pivotedData) {
            var row = { "Moneda": moneda };
            for (let categoria of categories) {
                row[categoria] = pivotedData[moneda][categoria] || 0;
            }
            row["Total"] = pivotedData[moneda]["Total"];
            row["ordenar"] = moneda === "Total" ? 999999 : 0; // Ordenamiento: 999999 para "Total" y 0 para las demás monedas
            result.push(row);
        }
        totalRow["ordenar"] = 999999; // asegurarse de que "Total" tiene el valor de ordenamiento más alto
        result.push(totalRow); // agregar la fila "Total" al final

        // agregar "Total" a las categorías para que se muestre como una columna en DataTables
        categories.add("Total");

        return {
            data: result,
            categories: Array.from(categories)
        };
    }


    function convertDataToObjectArray(dataObj) {
        var dataArray = [];

        for (var moneda in dataObj) {
            for (var categoria in dataObj[moneda]) {
                dataArray.push({
                    moneda: moneda,
                    categoria: categoria,
                    total: dataObj[moneda][categoria]
                });
            }
        }

        return dataArray;
    }


    $filtrarBusqueda.on('submit', function (event) {
        event.preventDefault();

        var fechaInicio = $('#fechaInicio').val();
        var fechaFin = $('#fechaFin').val();
        var almacen = $('#almacen').val();

        var fechaInicioDate = moment(fechaInicio, "DD/MM/YYYY").toDate();
        var fechaFinDate = moment(fechaFin, "DD/MM/YYYY").toDate();

        if (!isValidDate(fechaInicioDate) || !isValidDate(fechaFinDate) || !almacen) {
            Swal.fire({
                icon: 'error',
                title: 'Ups...',
                text: 'Por favor, complete todos los campos correctamente antes de realizar la búsqueda.',
            });
            return;
        }

        var formData = {
            fechaInicio: fechaInicio,
            fechaFin: fechaFin,
            almacen: almacen
        };

        var PaginaActual = $tablaVTipoPago.DataTable().page();
        formData.page = PaginaActual;

        $.ajax({
            url: '/AdminPages/filtrarTablaTP',
            type: 'POST',
            data: formData,
            success: function (data) {
                if (!data || Object.keys(data).length === 0) {
                    Swal.fire({
                        icon: 'error',
                        title: 'No se han registrado datos para este filtro',
                        text: 'Seleccione los campos donde si haya datos y vuelva a intentar',
                    });
                } else {
                    var convertedData = convertDataToObjectArray(data);
                    var pivoted = pivotData(convertedData);

                    var columnsConfig = [{ title: 'Moneda', data: 'Moneda' }];
                    pivoted.categories.forEach(cat => {
                        columnsConfig.push({ title: cat, data: cat });
                    });

                    // Agregar columna oculta para ordenar
                    columnsConfig.push({ title: 'Ordenar', data: 'ordenar', visible: false });

                    $tablaVTipoPago.DataTable().destroy();
                    $tablaVTipoPago.empty();
                    $tablaVTipoPago.DataTable({
                        data: pivoted.data,
                        columns: columnsConfig,
                        ordering: false,  // Deshabilitar el ordenamiento en todas las columnas
                        order: [[columnsConfig.length - 1, 'asc']], // Ordena por columna oculta "ordenar"
                        deferRender: true,
                        language: {
                            "sProcessing": "Procesando...",
                            "sLengthMenu": "Mostrar _MENU_ registros",
                            "sZeroRecords": "No se encontraron resultados",
                            "sEmptyTable": "Ningún dato disponible en esta tabla",
                            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                            "sInfoPostFix": "",
                            "sSearch": "Buscar:",
                            "sUrl": "",
                            "sInfoThousands": ",",
                            "sLoadingRecords": "Cargando...",
                            "oPaginate": {
                                "sFirst": "Primero",
                                "sLast": "Último",
                                "sNext": "Siguiente",
                                "sPrevious": "Anterior"
                            },
                            "oAria": {
                                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                            }
                        },

                        "dom": 'Blfrtip',
                        "lengthMenu": [[10, 50, 100, 1000, 2000, -1], [10, 50, 100, 1000, 2000, "Todos"]],
                        "pageLength": -1,

                        buttons: [

                            {
                                extend: 'excel',
                                className: 'btn btn-dark rounded-0 btn-excel',
                                text: '<i class="fas fa-file-excel"></i> Excel',
                                exportOptions: {
                                    columns: ':not(:eq(' + (columnsConfig.length - 1) + '))'  // Excluir la columna "Ordenar"
                                },
                                filename: function () {
                                    // Obtiene la fecha y hora actual.
                                    var now = new Date();

                                    // Formatea la fecha actual en dd-mm-yyyy.
                                    var day = ("0" + now.getDate()).slice(-2);
                                    var month = ("0" + (now.getMonth() + 1)).slice(-2);
                                    var year = now.getFullYear();
                                    var dateString = day + '-' + month + '-' + year;

                                    // Convierte la hora a formato de 12 horas con AM o PM.
                                    var hours = now.getHours();
                                    var minutes = now.getMinutes();
                                    var ampm = hours >= 12 ? 'PM' : 'AM';
                                    hours = hours % 12;
                                    hours = hours ? hours : 12; // La hora '0' debería ser '12'.
                                    minutes = minutes < 10 ? '0' + minutes : minutes;
                                    var timeString = hours + '-' + minutes + ' ' + ampm;

                                    // Obtiene la información del almacén seleccionado.
                                    var almacenSeleccionado = $('#almacen option:selected').text();
                                    var nombreAlmacen = almacenSeleccionado.includes('-') ? almacenSeleccionado : 'SinNombre';

                                    // Obtiene las fechas de inicio y fin seleccionadas y las formatea.
                                    var fechaInicio = $('#fechaInicio').val().replace(/\//g, '-');
                                    var fechaFin = $('#fechaFin').val().replace(/\//g, '-');

                                    // Formato del nombre del archivo: 'HOKA_REPORTE VENTA POR DEPARTAMENTO_NombreDelAlmacen_FechaInicio_FechaFin_FechaActual_HoraActual AM/PM.xlsx'
                                    return 'HOKA_REPORTE POR TIPO DE PAGO_SUCURSAL_' + nombreAlmacen + '_DEL_' + fechaInicio + '_AL_' + fechaFin + '_IMPRESO EL_' + dateString + '_CON HORA_' + timeString;
                                },
                                init: function (api, node, config) {
                                    $(node).css({
                                        "background-color": "green",
                                        "color": "white",
                                        "border": "double"
                                    });
                                },
                                footer: {
                                    columns: [3]
                                }
                            },


                            {
                                extend: 'pdf',
                                className: 'btn btn-dark rounded-0 btn-pdf',
                                text: '<i class="fas fa-file-pdf"></i> PDF',
                                title: null,
                                pageSize: 'A3',  // Establece el tamaño de página a A3
                                orientation: 'landscape',
                                exportOptions: {
                                    columns: ':not(:eq(' + (columnsConfig.length - 1) + '))'  // Excluir la columna "Ordenar"
                                },
                                filename: function () {
                                    // Obtiene la fecha actual en formato string.
                                    var now = new Date();
                                    var dateString = now.toISOString().slice(0, 10);

                                    // Obtiene la información del almacén seleccionado.
                                    var almacenSeleccionado = $('#almacen option:selected').text();
                                    var nombreAlmacen = almacenSeleccionado.includes('-') ? almacenSeleccionado : 'SinNombre';

                                    // Formato del nombre del archivo: 'HOKA - REPORTE VENTA POR PRODUCTO_NombreDelAlmacen_Fecha.pdf'
                                    return 'HOKA_REPORTE POR TIPO DE PAGO_SUCURSAL_' + nombreAlmacen + '_' + dateString;
                                },
                                customize: function (doc) {
                                    doc.defaultStyle.fontSize = 14; //tamaño de las letras dentro de la tablas al exportar

                                    // Obtiene la fecha y hora actuales.
                                    var currentDateTime = new Date();
                                    var currentDateTimeString = currentDateTime.toLocaleString();

                                    // Obtiene el rango de fechas ingresado.
                                    var fechaInicio = document.getElementById('fechaInicio').value;
                                    var fechaFin = document.getElementById('fechaFin').value;
                                    var fechaRango = 'DEL ' + fechaInicio + ' AL ' + fechaFin;

                                    // Obtiene la información del almacén seleccionado.
                                    var almacenSeleccionado = $('#almacen option:selected').text();
                                    var sucursal = 'SUCURSAL ' + almacenSeleccionado + ' - ' + fechaRango;

                                    // Fija el título del reporte a "HOKA - VENTA POR PRODUCTO".
                                    var tituloReporte = 'HOKA - REPORTE POR TIPO DE PAGO';
                                    var reporteDelDia = 'REPORTE DEL DÍA ' + currentDateTimeString;

                                    // Agrega el título en la parte superior del documento con texto en negrita.
                                    doc.content.splice(0, 0, {
                                        text: tituloReporte,
                                        alignment: 'center',
                                        margin: [0, 0, 20, 20],
                                        fontSize: 24,
                                        bold: true  // Texto en negrita
                                    });

                                    // Agrega la fecha del reporte y la información del almacén en la misma línea, alineados a la izquierda y a la derecha, respectivamente.
                                    doc.content.splice(1, 0, {
                                        columns: [
                                            {
                                                text: reporteDelDia,
                                                alignment: 'left',
                                                fontSize: 18
                                            },
                                            {
                                                text: sucursal,
                                                alignment: 'right',
                                                fontSize: 18
                                            }
                                        ],
                                        columnGap: 10  // Ajusta el espacio entre las columnas.
                                    });
                                },
                                init: function (api, node, config) {
                                    $(node).css({
                                        "background-color": "red",
                                        "color": "white",
                                        "border": "double"
                                    });
                                },
                                footer: {
                                    columns: [3]
                                },
                            },

                        ],


                        columnDefs: [
                            {
                                targets: '_all',
                                render: function (data, type, row, meta) {
                                    if (type === 'display' && typeof data === 'number') {
                                        return "$" + data.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                                    }
                                    return data;
                                }
                            }
                        ],


                    });
                    $(".btn-excel").css({
                        "background-color": "green",
                        "color": "white",
                        "border": "double"
                    });

                    $(".btn-pdf").css({
                        "background-color": "red",
                        "color": "white",
                        "border": "double"
                    });
                    // Aquí puedes agregar código adicional si necesitas añadir botones u otros elementos de estilo CSS.

                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error al filtrar: ' + errorThrown,
                });
            }
        });
    });

});
