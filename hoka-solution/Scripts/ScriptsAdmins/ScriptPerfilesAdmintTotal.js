

var modal = document.getElementById("ModalRegistro");
var btn = document.getElementById("abrirModalBoton");
var span = document.getElementsByClassName("close")[0];
var btnCerrarModal = document.getElementById("btn-cerrarmodal"); // Obtener referencia al botón de cerrar modal


// Función para limpiar el formulario
function limpiarFormulario() {
    var form = document.getElementById("form-registro");
    form.reset();

    // Reinicia Rol Select
    $('.rol-select').val('Selecciona un Rol').trigger('change');

    
}



btn.onclick = function () {
    modal.style.display = "block";
}

span.onclick = function () {
    modal.style.display = "none";
    limpiarFormulario(); // Llama a la función para limpiar el formulario
}

// Agregar evento al botón de cerrar modal
btnCerrarModal.onclick = function (event) {
    event.preventDefault(); // Prevenir que el formulario se envíe (porque es un botón submit)
    modal.style.display = "none";
    limpiarFormulario(); // Llama a la función para limpiar el formulario
}

window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
        limpiarFormulario(); // Llama a la función para limpiar el formulario
    }
}

//ESTO ES DENTRO DE LA VISTA DEL REGISTRO

$(document).ready(function () {

    

    $('#btn-registro').click(function (e) {
        e.preventDefault();

        $.ajax({
            url: '/AdminPages/RegistrarPerfil',
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
});



//SCRIPT PARA EL REGISTRO SOBRE LOS USUARIOS



$(document).ready(function () {

    // Inicializar Select2 Elements
    $('.select2').select2();

    // Inicializar Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });


});
