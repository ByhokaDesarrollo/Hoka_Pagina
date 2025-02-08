
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

    // para actualizar los datos completos del usuario

    $('#btn-editar1').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/AdminPages/EditarUsuario',
            type: 'POST',
            data: $('#form-editar1').serialize(),
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

    //Actualizar contraseña
    $('#btn-editar2').click(function (e) {
        e.preventDefault();

        // Primer cuadro de confirmación
        Swal.fire({
            title: '¿Estás seguro de que quieres cambiar la contraseña?',
            icon: 'question',
            showCancelButton: true,  // Muestra el botón de cancelar
            cancelButtonText: 'No, cancelar',
            confirmButtonText: 'Sí, cambiar',
            confirmButtonColor: '#28a745', // color verde para el botón de confirmación
            cancelButtonColor: '#ffc107',
            reverseButtons: true
        }).then((confirmation) => {
            if (confirmation.isConfirmed) {
                // Si el usuario confirma, realiza la petición AJAX
                $.ajax({
                    url: '/AdminPages/EditarContraseña',
                    type: 'POST',
                    data: $('#form-editar2').serialize(),
                    success: function (result) {
                        if (result.success) {
                            // Muestra el mensaje del servidor
                            Swal.fire({
                                icon: 'success',
                                title: 'Éxito',
                                text: result.message,
                                showConfirmButton: false,
                                timer: 1500
                            }).then(() => {
                                window.location.href = "/AdminPages/LUsuariosAdminTotal";
                            });
                        } else {
                            // Si hay un error, muestra el mensaje de error proveniente del servidor
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: result.message,
                                confirmButtonText: 'Aceptar'
                            });
                        }
                    }
                });
            }
        });
    });




});
