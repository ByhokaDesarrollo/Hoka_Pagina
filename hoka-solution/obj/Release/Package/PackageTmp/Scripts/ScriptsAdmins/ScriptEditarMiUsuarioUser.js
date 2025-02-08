
//este script solo es cuando necesitamos actualizar usuario
$(document).ready(function () {

    $("#FechaNacimientoU").datepicker({
        format: 'dd/mm/yyyy',
        language: 'es',
        autoclose: true,
        todayHighlight: true
    });

    $("#FechaNacimientoU").on("keyup", function () {
        var valor = $(this).val().replace(/\D/g, "").replace(/^(\d\d)(\d)/g, "$1/$2").replace(/\/(\d\d)(\d)/, "/$1/$2");
        $(this).val(valor.substring(0, 10));  // Limitar a 10 caracteres
    });

    $("#FechaNacimientoU").keypress(function (e) {
        // Permitir solo números y limitar a 10 caracteres (incluyendo '/')
        var charCode = (e.which) ? e.which : e.keyCode;
        if ($(this).val().length >= 10 && (charCode >= 48 && charCode <= 57)) {
            return false;
        } else if (charCode >= 48 && charCode <= 57) {
            return true;
        }
        return false;
    });

    // Evitar que se pegue texto no deseado
    $("#FechaNacimientoU").on('paste', function (e) {
        e.preventDefault();
    });



    $('#btn-editar1').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/AdminPages/EditarMiPerfilPersonalUser',
            type: 'POST',
            data: $('#form-editar1').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        text: result.message,
                        showConfirmButton: false,
                        timer: 5000
                    }).then((result) => {

                        window.location.href = "/UserPages/MiPerfilUser";
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

$(document).ready(function () {


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
        var checkbox = $('input[type="checkbox"]');

        // Si el rol seleccionado es el valor por defecto, deshabilitar y restablecer el campo de perfil y almacén
        if (selectedRol === "Selecciona un Rol") {
            perfilSelect.prop('disabled', true).val('Selecciona un Perfil').change();
            almacenSelect.prop('disabled', true).val('Selecciona un Almacen').change();
            checkbox.prop('disabled', true).prop('checked', false); // Deshabilita y desmarca el checkbox
        } else {
            // Restablecer el select de almacén a su estado por defecto
            almacenSelect.prop('disabled', true).val('Selecciona un Almacen').change();
            checkbox.prop('disabled', true).prop('checked', false); // Asegúrate de deshabilitar y desmarcar el checkbox

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
        var selectedRol = $('.rol-select-editar').val(); // Obtenemos el rol seleccionado
        var selectedPerfil = $(this).val();
        var almacenSelect = $('.almacen-select-editar');
        var checkbox = $('input[type="checkbox"]'); // Obtenemos la referencia del checkbox

        // Si el perfil seleccionado es el valor por defecto, deshabilitar y restablecer el campo de almacén
        if (selectedPerfil === "Selecciona un Perfil") {
            almacenSelect.prop('disabled', true).val('Selecciona un Almacen').change();
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
                        // Ignorar los almacenes que tengan nombreserver sea igual N
                        if (item.nombreserver !== 'N') {
                            almacenSelect.append($('<option>', {
                                value: item.Id_Almacen,
                                // Concatenar Almacen y Nombre con ' - ' entre ellos
                                text: item.Almacen + ' - ' + item.Nombre
                            }));
                        }
                    });

                    // Habilitar el select almacen
                    almacenSelect.prop('disabled', false);

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



    //comportamiento del checkbox que selecciona todo
    $('input[type="checkbox"]').click(function () {
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
    });

    selectAllWarehouses();

});
