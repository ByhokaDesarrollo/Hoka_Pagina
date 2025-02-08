
//este script solo es cuando necesitamos actualizar usuario
$(document).ready(function () {

   
    // para actualizar los datos completos del usuario

    $('#btn-editar2').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/AdminPages/EditarPermisos',
            type: 'POST',
            data: $('#form-editar2').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        text: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then((result) => {

                        window.location.href = "/AdminPages/LUsuariosAdminTotal";
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: result.message,
                        confirmButtonText: 'Aceptar'
                    });
                }
            }
        });
    });
});



function selectAllWarehouses() {
    $(".almacen-select-editar option").each(function () {
        if ($(this).text() != 'Selecciona un Almacen') {
            $(this).prop('selected', true);
        } else {
            $(this).prop('selected', false);
        }
    });
    // Actualizar la vista del componente Select2
    $(".almacen-select-editar").trigger('change');
}

function selectAllWarehouses1() {
    $(".permiso-select-editar option").each(function () {
        if ($(this).text() != 'Selecciona un Permiso') {
            $(this).prop('selected', true);
        } else {
            $(this).prop('selected', false);
        }
    });
    // Actualizar la vista del componente Select2
    $(".permiso-select-editar").trigger('change');
}

$(document).ready(function () {


    function areAllWarehousesSelected() {
        var almacenSelect = $('.almacen-select-editar');
        var allOptions = almacenSelect.find('option').not(':contains("Selecciona un Almacen")');
        var selectedOptions = almacenSelect.find('option:selected').not(':contains("Selecciona un Almacen")');

        return allOptions.length === selectedOptions.length;
    }


    function obtenerAlmacenes() {
        var almacenSelect = $('.almacen-select-editar');
        var selectedValues = almacenSelect.val(); // Guarda los almacenes seleccionados antes de la llamada
        var url = almacenSelect.data('url');

        $.ajax({
            url: '/AdminPages/ObtenerAlmacenes',
            type: 'GET',
            success: function (data) {
                almacenSelect.find('option').not(':first').remove();
                $.each(data, function (index, item) {
                    if (item.nombreserver !== 'N') {
                        almacenSelect.append($('<option>', {
                            value: item.Id_Almacen,
                            text: item.Almacen + ' - ' + item.Nombre
                        }));
                    }
                });
                // Restaurar los valores seleccionados después de actualizar las opciones
                almacenSelect.val(selectedValues).trigger('change');

                // Verifica si todos los almacenes están seleccionados
                if (areAllWarehousesSelected()) {
                    $('.checkboxAlmacen-select').prop('checked', true);
                } else {
                    $('.checkboxAlmacen-select').prop('checked', false);
                }
            },
            error: function () {
                // Manejar el error de la solicitud AJAX si es necesario
            }
        });
    }

    function obtenerPerfiles() {
        var perfilSelect = $('.perfil-select-editar');
        var selectedValues = perfilSelect.val(); // Guarda los perfiles seleccionados antes de la llamada
        var url = perfilSelect.data('url');
        var rolId = $('.rol-select-editar').val();  // Obtener el valor del Rol

        $.ajax({
            url: '/AdminPages/ObtenerPerfiles',
            type: 'GET',
            data: {
                idRol: selectedValues ? selectedValues : rolId // Si el perfil es null, usa el valor del rol
            },
            success: function (data) {
                perfilSelect.find('option').not(':first').remove();
                $.each(data, function (index, item) {
                    perfilSelect.append($('<option>', {
                        value: item.Id_Perfil,
                        text: item.Nombre_Perfil
                    }));
                });

                // Restaurar los valores seleccionados después de actualizar las opciones
                if (selectedValues) {
                    perfilSelect.val(selectedValues).trigger('change');
                } else {
                    perfilSelect.val(null).trigger('change');  // Si el perfil estaba en null, ponlo en null de nuevo
                }
            },
            error: function () {
                alert('Error al cargar los perfiles.');
            }
        });
    }

    // Inicializar Select2 Elements
    $('.select2').select2();

    // Inicializar Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });

    // Al seleccionar un rol
    $('.rol-select-editar').change(function () {
        var selectedRol = $(this).val();
        var perfilSelect = $('.perfil-select-editar');
        var almacenSelect = $('.almacen-select-editar');
        var checkbox = $('.checkboxAlmacen-select');

        if (selectedRol === "Selecciona un Rol") {
            // ... (mantén el código actual aquí) ...
        } else {
            if (selectedRol == '2') { // Si el rol seleccionado es "User"
                // Restablecer el select de almacén a su estado por defecto
                almacenSelect.prop('disabled', false).val('Selecciona un Almacen').change();
                checkbox.prop('disabled', true).prop('checked', false); // Asegúrate de deshabilitar y desmarcar el checkbox
            }
            // Obtener la URL del controlador para obtener perfiles
            var url = perfilSelect.data('url');

            // Realizar solicitud AJAX para obtener perfiles basados en el rol seleccionado
            $.ajax({
                url: '/AdminPages/ObtenerPerfiles',
                type: 'GET',
                data: { idRol: selectedRol },
                success: function (data) {
                    // Limpiar las opciones existentes
                    perfilSelect.find('option').not(':first').remove();

                    // Agregar las opciones obtenidas de la respuesta AJAX
                    $.each(data, function (index, item) {
                        perfilSelect.append($('<option>', {
                            value: item.Id_Perfil,
                            text: item.Nombre_Perfil
                        }));
                    });

                    // Habilitar el select de perfil
                    perfilSelect.prop('disabled', false);

                    // Cambiar el comportamiento del select de almacén y del checkbox según el rol seleccionado
                    if (selectedRol == '2') { // Si el rol seleccionado es "User"
                        // Deshabilitar y desmarcar el checkbox
                        checkbox.prop('disabled', true).prop('checked', false);
                    }
                },
                error: function () {
                    // Manejar el error de la solicitud AJAX si es necesario
                }
            });
        }
    });
    // Al seleccionar un perfil
    $('.perfil-select-editar').change(function () {
        var selectedRol = $('.rol-select-editar').val();
        var selectedPerfil = $(this).val();
        var almacenSelect = $('.almacen-select-editar');
        var checkbox = $('.checkboxAlmacen-select');
        var selectedAlmacenes = almacenSelect.val(); // Guardamos los almacenes seleccionados

        // Si el perfil seleccionado es el valor por defecto, sólo deshabilitamos el checkbox
        if (selectedPerfil === "Selecciona un Perfil") {
            checkbox.prop('disabled', true); // Siempre deshabilitamos el checkbox si el perfil es el valor por defecto
        } else {
            // Asegurarse de que la opción "Selecciona un Almacén" está presente
            if (almacenSelect.find('option[value="Selecciona un Almacen"]').length === 0) {
                almacenSelect.append(new Option('Selecciona un Almacen', 'Selecciona un Almacen', true, true));
            }

            // Obtener la URL del controlador para obtener almacenes
            var url = almacenSelect.data('url');

            // Realizar solicitud AJAX para obtener ciudades basadas en el perfil seleccionado
            $.ajax({
                url: '/AdminPages/ObtenerAlmacenes',
                type: 'GET',
                success: function (data) {
                    // Limpiar las opciones existentes
                    almacenSelect.find('option').not(':first').remove();

                    // Agregar las opciones obtenidas de la respuesta AJAX
                    $.each(data, function (index, item) {
                        if (item.nombreserver !== 'N') {
                            almacenSelect.append($('<option>', {
                                value: item.Id_Almacen,
                                text: item.Almacen + ' - ' + item.Nombre
                            }));
                        }
                    });



                    // Restablecemos las opciones seleccionadas previamente
                    almacenSelect.val(selectedAlmacenes);

                    // Habilitar el checkbox sólo si el rol es 'Admin' o 'Admin Total' y se ha seleccionado un perfil válido
                    if ((selectedRol == '1' || selectedRol == '3') && selectedPerfil !== "Selecciona un Perfil") {
                        checkbox.prop('disabled', false);
                    } else {
                        checkbox.prop('disabled', true);
                    }
                },
                error: function () {
                    // Manejar el error de la solicitud AJAX si es necesario
                }
            });
        }
    });
    // Al cambiar la selección en el select de almacen
    $('.almacen-select-editar').change(function () {
        var selectedRol = $('.rol-select-editar').val();
        var selectedValues = $(this).val();

        // Si se deselecciona todo, agregar la opción "Selecciona un Almacen" de nuevo
        if (selectedValues.length === 0 || selectedValues === null) {
            if ($(this).find('option:contains("Selecciona un Almacen")').length == 0) {
                $(this).append('<option selected="selected">Selecciona un Almacen</option>');
            }
        }

        // Si se seleccionó algo distinto de "Selecciona un Almacen", eliminar la opción
        else if ($.inArray('Selecciona un Almacen', selectedValues) > -1 && selectedValues.length > 1) {
            $(this).find('option:selected').each(function () {
                if ($(this).text() === 'Selecciona un Almacen') {
                    $(this).remove();
                }
            });
        }

        // Si el rol seleccionado es "User"
        else if (selectedRol == '2') {
            // Limitar el número de opciones seleccionables a 1
            if (selectedValues.length > 1) {
                $(this).val(selectedValues[0]); // Mantener siempre la primera opción seleccionada
                $(this).trigger('change.select2'); // Esto obligará a Select2 a actualizar el desplegable
            }
        }

        // Actualizar la vista del componente Select2
        $(this).trigger('change.select2');
    });
    //comportamiento del checkbox almacenes que selecciona todo
    $('.checkboxAlmacen-select').click(function () {
        if ($(this).is(':checked')) {
            // Si el checkbox está marcado, seleccionar todas las opciones del select de almacén
            // excepto la opción "Selecciona un Almacen"
            $(".almacen-select-editar option").each(function () {
                if ($(this).text() != 'Selecciona un Almacen') {
                    $(this).prop('selected', true);
                } else {
                    $(this).prop('selected', false);
                }
            });
        } else {
            // Si el checkbox no está marcado, deseleccionar todas las opciones del select de almacén
            $(".almacen-select-editar option").prop('selected', false);
            // Y seleccionar la opción "Selecciona un Almacen"
            $(".almacen-select-editar option").each(function () {
                if ($(this).text() == 'Selecciona un Almacen') {
                    $(this).prop('selected', true);
                }
            });
        }
        // Actualizar la vista del componente Select2
        $(".almacen-select-editar").trigger('change');

        if (areAllWarehousesSelected()) {
            checkbox.prop('checked', true);
        } else {
            checkbox.prop('checked', false);
        }
    });

    $('.load-almacen').click(function () {
        obtenerAlmacenes();
    });

    $('.load-perfil').click(function () {
        obtenerPerfiles();
    });

    selectAllWarehouses();

    //Comportamiento del campo permiso

    let $permisoSelect = $('.permiso-select-editar');
    let $selectAllCheckbox = $('.checkboxPermiso-select');

    $('.load-permiso').click(function (event) {
        event.stopPropagation();
        event.preventDefault();

        // Obtener los permisos actualmente seleccionados
        let permisosSeleccionados = $('.permiso-select-editar').val();

        $.ajax({
            type: "GET",
            url: "/AdminPages/ObtenerPermisos",
            dataType: "json",
            success: function (data) {
                $('.permiso-select-editar').empty(); // Limpia el select
                $('.permiso-select-editar').append('<option>Selecciona un Permiso</option>'); // Agrega la opción por defecto
                let allPermisos = [];

                $.each(data, function (index, permiso) {
                    $('.permiso-select-editar').append('<option value="' + permiso.Id_Permiso + '">' + permiso.Nombre_Permiso + '</option>');
                    allPermisos.push(permiso.Id_Permiso.toString());
                });

                // Selecciona solamente los permisos previamente seleccionados
                $('.permiso-select-editar').val(permisosSeleccionados).trigger('change.select2'); // Actualiza la vista de Select2
            },
            error: function (error) {
                console.log("Error al obtener los permisos: ", error);
            }
        });
    });

    function allOptionsSelected() {
        let totalOptions = $permisoSelect.children('option').length;
        // Cuando "Selecciona un Permiso" existe, debemos descontar una opción del total
        if ($permisoSelect.children('option').text().includes("Selecciona un Permiso")) {
            totalOptions -= 1;
        }
        let selectedOptions = $permisoSelect.val() ? $permisoSelect.val().length : 0;
        return totalOptions === selectedOptions;
    }

    function ensureDefaultOption() {
        if (!$permisoSelect.children('option').text().includes("Selecciona un Permiso")) {
            $permisoSelect.prepend('<option>Selecciona un Permiso</option>');
        }
    }

    function removeDefaultOption() {
        $permisoSelect.find('option').each(function () {
            if ($(this).text() === "Selecciona un Permiso") {
                $(this).remove();
            }
        });
    }

    $permisoSelect.on('change', function () {
        let selectedValues = $(this).val();

        if (selectedValues && selectedValues.length > 0) {
            removeDefaultOption();
        } else {
            ensureDefaultOption();
            $permisoSelect.val('Selecciona un Permiso').trigger('change.select2');
        }

        $selectAllCheckbox.prop('checked', allOptionsSelected());
    });

    $('.checkboxPermiso-select').click(function () {
        if ($(this).is(':checked')) {
            $permisoSelect.find('option').prop('selected', true);
            removeDefaultOption();
        } else {
            $permisoSelect.val(null);
            ensureDefaultOption();
            $permisoSelect.val('Selecciona un Permiso');
        }
        $permisoSelect.trigger('change.select2');
    });
    selectAllWarehouses1();
});
