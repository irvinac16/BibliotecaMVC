// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $(document).on('submit', 'form.swal-delete-form', function(e){
        e.preventDefault();
        var form = this;
        var tipo = $(form).attr('data-tipo');
        Swal.fire({
            title: "¿Eliminar " + tipo + "?",
            text: "Esta accion no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Si, eliminar",
            cancelButtonText: "Cancelar"
        })
        .then(function(result){
            if (result.isConfirmed) {
                form.submit();
            }
        });
    });

    $(document).on('submit', 'form.swal-save-form', function (e) {
        e.preventDefault();
        var form = this;
        Swal.fire({
            title: "¿Desea guardar los cambios?",
            text: "¿Estas seguro de que quieres guardar los cambios?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Si, guardar",
            cancelButtonText: "Cancelar"
        })
            .then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
    });
});
