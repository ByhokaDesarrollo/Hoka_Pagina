function AlertaBorrarUsuario(Identificador) {
    Swal.fire({
        title: '¿Esta seguro de eliminar este perfil?',
        icon: 'error',
        butttons: true,
        showCancelButton: true,
        dangerMode: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/AdminPages/EliminarPerfil?IdPerfil=" + Identificador,
                type: "POST",
                success: function (r) {
                    window.location = "/AdminPages/PerfilesForUsersAdminTotal";
                }
            });
        }
        return false;
    });
}