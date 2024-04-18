let datatable;
$(document).ready(function () {
    loadDataTable();  //Sirve para leer la tabla
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        //Seccion de ajax para el´pluggin
        "ajax": { "url": "/Admin/Categoria/obtenerTodos" },
        "columns": [
            { "data": "nombre", "width": "20%" },
            { "data": "descripcion", "width": "40%" },
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) {
                        return "Activo";
                    }
                    else {
                        return "Inactivo";
                    }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {  //Alt + 96 para estos simbolos
                    return ` 
                        <div class="text-center">
                            <a href="/Admin/Categoria/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick=Delete("/Admin/Categoria/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                            <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                }, "width": "20%"
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json'
        }
    });
}

function Delete(url) {
    swal({
        title: "¿Estás seguro de eliminar la Categoria?",
        text: "Este registro no será recuperado",
        icon: "warning",
        buttons: true,
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "Post",
                url: url,
                success: function (data) {
                    if (dara.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }

            });
        }
    });
}