$(document).ready(function () {
    //Aqui empieza la tabla
    var $filtrarBusqueda = $('#filtrarBusqueda');
    var $tablaRemisiod = $('#tablaRemisiod');

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

    $tablaRemisiod.DataTable({
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
        dom: '<"top"Bfl>rt<"bottom"ip>',
        buttons: [
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel"></i> Excel',
                className: 'btn btn-excel-brillante', // Clase personalizada
                title: 'Reporte Tickets',
                exportOptions: {
                    columns: ':visible',
                    modifier: {
                        page: 'all'
                    }
                }
            }
        ],
        "lengthMenu": [
            [10, 50, 100, 1000, 2000, -1],
            [10, 50, 100, 1000, 2000, "Todos"]
        ],
        "pageLength": -1,

        columns: [
            { data: 'folio_remision' },
            { data: 'almacen' },
            {
                data: 'tipo_cambio',
                render: function (data, type, row) {
                    return parseFloat(data).toLocaleString('es-MX', {
                        style: 'currency',
                        currency: 'MXN'
                    });
                }
            },
            {
                data: 'stotal',
                render: function (data, type, row) {
                    return parseFloat(data).toLocaleString('es-MX', {
                        style: 'currency',
                        currency: 'MXN'
                    });
                }
            },
            {
                data: 'fecha',
                render: function (data, type, row) {
                    return moment(data).utc().format('DD/MM/YYYY');
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `<a href="/AdminPages/DetalleTicketAdminTotal?idFolio=${row.folio_remision}" target="_blank" class="btn btn-grey"><i class="fas fa-info-circle"></i> Detalle</a>`;
                },
                orderable: false
            },
        ],

        deferRender: true, // Se habilita el renderizado diferido

        drawCallback: function (settings) {
            // Tu código de callback si es necesario
        },

        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            function roundToTwoDecimals(number) {
                return +(Math.round(number + "e+2") + "e-2");
            }

            var sumaVentaTotal = api.column(3, { page: 'current' }).data().reduce(function (a, b) { return a + (parseFloat(b) || 0); }, 0);

            sumaVentaTotal = roundToTwoDecimals(sumaVentaTotal);

            $('#sumaVentaTotal').html('Total Venta Real: $' + sumaVentaTotal.toLocaleString('es-MX'));
        },
    });

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

        var PaginaActual = $tablaRemisiod.DataTable().page();
        formData.page = PaginaActual;

        $.ajax({
            url: '/AdminPages/filtrarTablaCT',
            type: 'POST',
            data: formData,
            success: function (data) {
                if (!data || data.length === 0) {
                    Swal.fire({
                        icon: 'error',
                        title: 'No se han registrado datos para este filtro',
                        text: 'Seleccione los campos donde si haya datos y vuelva a intentar',
                    });
                } else {
                    $tablaRemisiod.DataTable().clear().rows.add(data).draw();
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

    $tablaRemisiod.on('length.dt', function (e, settings, len) {
        // Tu código de callback si es necesario
    });
});