// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function createToast(type, text, height=75) {
    var header = "Error";
    if (type === "success") {
        header = "Success"
    }
    $("#toaster").prepend(`
        <div aria-live="polite" aria-atomic="true" class="d-flex justify-content-center" style="min-height: ${height}px; position: relative;">
            <div id="toast-generic" class="toast" role="${type}" aria-live="assertive" aria-atomic="true">
                <div class="toast-header">
                    <strong class="mr-auto">${header}</strong>
                    <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="toast-body">
                    ${text}
                </div>
            </div>
        </div>
    `);

    $(".toast").toast({ delay: 7000 });
    $('.toast').on('hidden.bs.toast', function () {
        $(this).parent().remove();
    })
    $(".toast").toast('show');
}