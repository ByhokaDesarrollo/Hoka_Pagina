$(document).ready(function () {
    $('#btn-login').click(function (e) {
        e.preventDefault();

        $.ajax({
            url: '/Acceso/Login',
            type: 'POST',
            data: $('#form-login').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then(() => {
                        // Acceder directamente a result.redirectToUrl en el bloque then
                        window.location.href = result.redirectToUrl;
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Algo está mal...',
                        text: result.message,
                        confirmButtonText: 'Aceptar'
                    });
                }
            }
        });
    });
});





