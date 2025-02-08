$(document).ready(function () {

    //Aqui empieza la tabla


    var $formRotacion = $('#formRotacion');
    var $tablaProdNoRotados = $('#tablaProdNoRotados');

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


    $tablaProdNoRotados.DataTable({
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

        "lengthMenu": [
            [10, 50, 100, 1000, 2000, -1],
            [10, 50, 100, 1000, 2000, "Todos"]
        ],
        "pageLength": -1,

        columns: [
            {
                data: 'codigobarras',
                render: function (data, type, row) {
                    return data ? data : 'Sin registro';
                }
            },
            {
                data: 'productoNombre',
                render: function (data, type, row) {
                    return data ? data : 'Sin registro';
                }
            },
            {
                data: 'almacen',
                render: function (data, type, row) {
                    return data ? data : 'Sin registro';
                }
            },
            {
                data: 'categoria',
                render: function (data, type, row) {
                    return data ? data : 'Sin registro';
                }
            },
            {
                data: 'cantidads',
                render: function (data, type, row) {
                    return data ? data : '0';
                }
            },
            {
                data: 'Existencia',
                render: function (data, type, row) {
                    return data ? data : 'Sin registro';
                }
            },
            {
                data: 'fecha',
                render: function (data, type, row) {
                    return data ? moment(data).utc().format('DD/MM/YYYY') : 'Sin Registro';
                }
            },
            


        ],

        deferRender: true, // Se habilita el renderizado diferido

        drawCallback: function (settings) {

        },

        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            function roundToTwoDecimals(number) {
                return +(Math.round(number + "e+2") + "e-2");
            }

            var sumaCantidads = api.column(4, { page: 'current' }).data().reduce(function (a, b) { return a + (parseFloat(b) || 0); }, 0);
            var sumaExistencia = api.column(5, { page: 'current' }).data().reduce(function (a, b) { return a + (parseFloat(b) || 0); }, 0);

            sumaCantidads = roundToTwoDecimals(sumaCantidads);
            sumaExistencia = roundToTwoDecimals(sumaExistencia);

            $('#sumaCantidads').html('Total Unidades: ' + sumaCantidads.toLocaleString('es-MX'));
            $('#sumaExistencia').html('Total Existencia: ' + sumaExistencia.toLocaleString('es-MX'));

        },
    });

    $formRotacion.on('submit', function (event) {  // Cambia ID_DEL_FORMULARIO al id de tu formulario
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

        var PaginaActual = $tablaProdNoRotados.DataTable().page();
        formData.page = PaginaActual;

        console.log("Enviando datos:", formData); // Muestra los datos que estás enviando

        if ($("#radioProductosRotados").prop("checked")) {
            $.ajax({
                url: '/AdminPages/filtrarTablaRotacion',
                type: 'POST',
                data: formData,
                success: function (data) {
                    /*console.log("Respuesta del servidor:", data); // Muestra la respuesta del servidor*/
                    
                    
                    $tablaProdNoRotados.DataTable().column(6).visible(true);  // Mostrar la columna 'categoria'
                    $tablaProdNoRotados.DataTable().column(3).visible(true);  // Mostrar la columna 'categoria'
                    $tablaProdNoRotados.DataTable().column(4).visible(true);  // Mostrar la columna 'Uni. Vendidas Periodo'
                    if (!data || data.length === 0) {
                        Swal.fire({
                            icon: 'error',
                            title: 'No se han registrado datos para este filtro',
                            text: 'Seleccione los campos donde si haya datos y vuelva a intentar',
                        });
                        
                    } else {
                        $tablaProdNoRotados.DataTable().clear().rows.add(data).draw();
                        
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                   /* console.log("Error en la solicitud:", errorThrown); // Muestra errores*/
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Error al filtrar: ' + errorThrown,
                    });
                }
            });
        } else if ($("#radioProductosNoRotados").prop("checked")) {
            $.ajax({
                url: '/AdminPages/filtrarTablaRotacionNull',
                type: 'POST',
                data: formData,
                success: function (data) {
                    console.log("Respuesta del servidor:", data); // Muestra la respuesta del servidor
                    
                    $tablaProdNoRotados.DataTable().column(6).visible(false); // Ocultar la columna 'categoria'
                    $tablaProdNoRotados.DataTable().column(3).visible(false); // Ocultar la columna 'categoria'
                    $tablaProdNoRotados.DataTable().column(4).visible(false); // Ocultar la columna 'Uni. Vendidas Periodo'
                    if (!data || data.length === 0) {
                        Swal.fire({
                            icon: 'error',
                            title: 'No se han registrado datos para este filtro',
                            text: 'Seleccione los campos donde si haya datos y vuelva a intentar',
                        });
                        
                    } else {
                        $tablaProdNoRotados.DataTable().clear().rows.add(data).draw();
                        
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Error en la solicitud:", errorThrown); // Muestra errores
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Error al filtrar: ' + errorThrown,
                    });
                }
            });
        }
    });


    
    $tablaProdNoRotados.on('length.dt', function (e, settings, len) {

    });
});