function AlertaBorrarUsuario(Identificador) {
    Swal.fire({
        title: '¿Esta seguro de eliminar al Usuario?',
        icon: 'error',
        butttons: true,
        showCancelButton: true,
        dangerMode: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/AdminPages/EliminarUsuario?IdUsuario=" + Identificador,
                type: "POST",
                success: function (r) {
                    window.location = "/AdminPages/LUsuariosAdminTotal";
                }
            });
        }
        return false;
    });
}