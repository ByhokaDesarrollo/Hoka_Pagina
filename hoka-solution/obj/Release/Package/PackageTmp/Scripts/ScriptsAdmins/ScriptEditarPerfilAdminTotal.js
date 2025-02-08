
//este script solo es cuando necesitamos actualizar usuario
$(document).ready(function () {

    $('#btn-editar').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/AdminPages/EditarPerfil',
            type: 'POST',
            data: $('#form-editar').serialize(),
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        text: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then((result) => {

                        window.location.href = "/AdminPages/PerfilesForUsersAdminTotal";
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

    // Inicializar Select2 Elements
    $('.select2').select2();

    // Inicializar Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });

});