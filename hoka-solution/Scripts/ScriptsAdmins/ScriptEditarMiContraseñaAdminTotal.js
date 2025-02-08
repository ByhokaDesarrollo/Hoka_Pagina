//este script solo es cuando necesitamos actualizar contraseña
$(document).ready(function () {
    $('#btn-editarcontra').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/AdminPages/EditarMiContraseña',
            type: 'POST',
            data: $('#form-editarcontra').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        text: result.message,
                        showConfirmButton: false,
                        timer: 5000
                    }).then((result) => {

                        window.location.href = "/Acceso/CerrarSesion";
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