// validar el acceso para la vista (un bloqueo de pantalla)
/*$(function () {
    // Obtener el enlace
    const enlace = document.getElementById("accesoRe");

    // Agregar un evento de click al enlace
    enlace.addEventListener("click", function (event) {
        event.preventDefault(); // Prevenir que se abra el enlace inmediatamente

        // Mostrar el SweetAlert2 para solicitar la contraseña
        Swal.fire({
            title: "Ingrese la contraseña",
            input: "password",
            showCancelButton: true,
            confirmButtonText: "Acceder",
            cancelButtonText: "Cancelar",
            preConfirm: (password) => {
                // Hacer una solicitud de AJAX al servidor para validar la contraseña
                return $.ajax({
                    url: "/Acceso/ValidarAcceso",
                    type: "POST",
                    data: { password: password },
                    dataType: "json"
                }).then((response) => {
                    if (response.valid) {
                        // Si la contraseña es correcta, permitir el acceso al enlace
                        window.location.href = "/Acceso/Registro";
                    } else {
                        // Si la contraseña es incorrecta, mostrar un mensaje de error
                        throw new Error("Contraseña incorrecta");
                        Swal.fire({
                            icon: 'error',
                            title: 'La contraseña es incorrecta',
                            text: 'Contactese con el administrador',
                            footer: '<a href="">Why do I have this issue?</a>'
                        })
                    }
                });
            }
        });
    });
});
*/

//Lo que pasa despues del registro

$(document).ready(function () {
    $('#btn-registro').click(function (e) {
        e.preventDefault();

        $.ajax({
            url: '/Acceso/Registro',
            type: 'POST',
            data: $('#form-registro').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        text: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then((result) => {

                        window.location.href = "/Acceso/Login";
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



//SCRIPT PARA EL REGISTRO SOBRE LOS USUARIOS



$(document).ready(function () {
    // Al seleccionar un rol
    $('.rol-select').change(function () {
        var selectedRol = $(this).val();
        var perfilSelect = $('.perfil-select');

        // Deshabilitar y restablecer el select de perfil
        perfilSelect.prop('disabled', true).val('').change();

        // Obtener la URL del controlador para obtener perfiles
        var url = perfilSelect.data('url');

        // Realizar solicitud AJAX para obtener perfiles basados en el rol seleccionado
        $.ajax({
            url: '/Acceso/ObtenerPerfiles',
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
            },
            error: function () {
                // Manejar el error de la solicitud AJAX si es necesario
            }
        });
    });

    // Al seleccionar un perfil
    $('.perfil-select').change(function () {
        var selectedPerfil = $(this).val();
        var ciudadSelect = $('.ciudad-select');

        // Deshabilitar y restablecer el select de ciudad
        ciudadSelect.prop('disabled', true).val('').change();

        // Obtener la URL del controlador para obtener ciudades
        var url = ciudadSelect.data('url');

        // Realizar solicitud AJAX para obtener ciudades basadas en el perfil seleccionado
        $.ajax({
            url: '/Acceso/ObtenerCiudades',
            type: 'GET',
            data: { idPerfil: selectedPerfil },
            success: function (data) {
                // Limpiar las opciones existentes
                ciudadSelect.find('option').not(':first').remove();

                // Agregar las opciones obtenidas de la respuesta AJAX
                $.each(data, function (index, item) {
                    // Ignorar la ciudad con Id_Ciudad = 5
                    if (item.Id_Ciudad !== 5) {
                        ciudadSelect.append($('<option>', {
                            value: item.Id_Ciudad,
                            text: item.Nombre_Ciudad
                        }));
                    }
                });

                // Habilitar el select de ciudad
                ciudadSelect.prop('disabled', false);
            },
            error: function () {
                // Manejar el error de la solicitud AJAX si es necesario
            }
        });
    });


    // Al seleccionar una ciudad
    $('.ciudad-select').change(function () {
        var selectedCiudad = $(this).val();
        var tiendaSelect = $('.tienda-select');

        // Deshabilitar y restablecer el select de tienda
        tiendaSelect.prop('disabled', true).val('').change();

        // Obtener la URL del controlador para obtener tiendas
        var url = tiendaSelect.data('url');

        // Realizar solicitud AJAX para obtener tiendas basadas en la ciudad seleccionada
        $.ajax({
            url: '/Acceso/ObtenerTiendas',
            type: 'GET',
            data: { idCiudad: selectedCiudad },
            success: function (data) {
                // Limpiar las opciones existentes
                tiendaSelect.find('option').not(':first').remove();

                // Agregar las opciones obtenidas de la respuesta AJAX
                $.each(data, function (index, item) {
                    tiendaSelect.append($('<option>', {
                        value: item.Id_Tienda,
                        text: item.Nombre_Tienda
                    }));
                });

                // Habilitar el select de tienda
                tiendaSelect.prop('disabled', false);
            },
            error: function () {
                // Manejar el error de la solicitud AJAX si es necesario
            }
        });
    });
});